using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public enum TutorialStep
{
    Aim,           // Animated hand swipe + "Glisse pour viser !"
    Shoot,         // "Relache !" — dismissed on OnShoot
    Bounce,        // Hidden, waits for wall bounce → flash 2s
    CounterAttack, // Hidden, waits for first damage → flash 2s
    Upgrade,       // Hidden, waits for upgrade screen → flash 2s
    Done
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private TutorialStep _step = TutorialStep.Aim;
    private Canvas _canvas;
    private CanvasGroup _bubbleGroup;
    private TextMeshProUGUI _text;
    private RectTransform _handRect;
    private Image _handImage;
    private bool _active;

    private Coroutine _handAnim;
    private Coroutine _autoDismiss;
    private Coroutine _fadeRoutine;

    private BowController _bow;
    private BowHealth _bowHealth;
    private UpgradeManager _upgradeMgr;
    private TMP_FontAsset _showpopFont;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.TutorialDone)
        {
            Destroy(this);
            return;
        }

        _bow = FindFirstObjectByType<BowController>();
        _bowHealth = FindFirstObjectByType<BowHealth>();
        _upgradeMgr = UpgradeManager.Instance;
        _showpopFont = Resources.Load<TMP_FontAsset>("UI/Showpop SDF");

        CreateUI();
        SubscribeEvents();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            if (GameManager.Instance.CurrentState == GameState.Playing)
                OnGameStateChanged(GameState.Playing);
        }
    }

    void OnDestroy()
    {
        UnsubscribeEvents();
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
        if (Instance == this) Instance = null;
    }

    // ─── UI CREATION ───

    void CreateUI()
    {
        // Canvas (sortOrder 45 — above HUD but below upgrade/gameover screens)
        GameObject canvasGO = new GameObject("TutorialCanvas");
        canvasGO.transform.SetParent(transform);
        _canvas = canvasGO.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 45;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Hint bubble (compact, rounded, bottom area) ──
        GameObject bubbleGO = new GameObject("HintBubble");
        bubbleGO.transform.SetParent(canvasGO.transform, false);
        _bubbleGroup = bubbleGO.AddComponent<CanvasGroup>();
        _bubbleGroup.blocksRaycasts = false;
        _bubbleGroup.alpha = 0f;

        // Rounded bubble background
        Sprite panelSprite = Resources.Load<Sprite>("UI/RoundedPanel");
        var bubbleBg = bubbleGO.AddComponent<Image>();
        if (panelSprite != null)
        {
            bubbleBg.sprite = panelSprite;
            bubbleBg.type = Image.Type.Sliced;
        }
        bubbleBg.color = new Color(0.06f, 0.06f, 0.14f, 0.88f);
        bubbleBg.raycastTarget = false;
        var bubbleRect = bubbleGO.GetComponent<RectTransform>();
        bubbleRect.anchorMin = new Vector2(0.5f, 0f);
        bubbleRect.anchorMax = new Vector2(0.5f, 0f);
        bubbleRect.pivot = new Vector2(0.5f, 0f);
        bubbleRect.anchoredPosition = new Vector2(0, 340);
        bubbleRect.sizeDelta = new Vector2(700, 90);

        // Hint text (Showpop font)
        GameObject textGO = new GameObject("HintText");
        textGO.transform.SetParent(bubbleGO.transform, false);
        _text = textGO.AddComponent<TextMeshProUGUI>();
        _text.fontSize = 48;
        _text.alignment = TextAlignmentOptions.Center;
        _text.color = Color.white;
        _text.enableWordWrapping = true;
        _text.fontStyle = FontStyles.Bold;
        _text.outlineWidth = 0.15f;
        _text.outlineColor = new Color32(0, 0, 0, 220);
        _text.raycastTarget = false;
        if (_showpopFont != null) _text.font = _showpopFont;
        RectFill(textGO);

        // ── Animated hand sprite ──
        Sprite handSprite = Resources.Load<Sprite>("UI/TutorialHand");

        GameObject handGO = new GameObject("HandIcon");
        handGO.transform.SetParent(canvasGO.transform, false);
        _handImage = handGO.AddComponent<Image>();
        _handImage.raycastTarget = false;
        if (handSprite != null)
        {
            _handImage.sprite = handSprite;
            _handImage.preserveAspect = true;
        }
        _handImage.color = new Color(1f, 1f, 1f, 0.9f);
        _handRect = handGO.GetComponent<RectTransform>();
        _handRect.sizeDelta = new Vector2(120, 120);
        _handImage.gameObject.SetActive(false);

        HideAll();
    }

    void RectFill(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    // ─── EVENTS ───

    void SubscribeEvents()
    {
        if (_bow != null) _bow.OnAimStarted += OnAimStarted;
        if (_bow != null) _bow.OnShoot += OnShoot;
        ArrowCollisionHandler.OnWallBounce += OnWallBounce;
        if (_bowHealth != null) _bowHealth.OnDamageTaken += OnDamageTaken;
        if (_upgradeMgr != null) _upgradeMgr.OnUpgradesOffered += OnUpgradesOffered;
    }

    void UnsubscribeEvents()
    {
        if (_bow != null) _bow.OnAimStarted -= OnAimStarted;
        if (_bow != null) _bow.OnShoot -= OnShoot;
        ArrowCollisionHandler.OnWallBounce -= OnWallBounce;
        if (_bowHealth != null) _bowHealth.OnDamageTaken -= OnDamageTaken;
        if (_upgradeMgr != null) _upgradeMgr.OnUpgradesOffered -= OnUpgradesOffered;
    }

    void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Playing && !_active && _step == TutorialStep.Aim)
        {
            _active = true;
            ShowStep(TutorialStep.Aim);
        }
    }

    // ─── EVENT HANDLERS (flexible: skip ahead if a later event fires early) ───

    void OnAimStarted()
    {
        if (_step != TutorialStep.Aim) return;
        AdvanceTo(TutorialStep.Shoot);
    }

    void OnShoot(Vector2 dir, float speed)
    {
        if (_step > TutorialStep.Shoot) return;
        AdvanceTo(TutorialStep.Bounce);
    }

    void OnWallBounce()
    {
        if (_step > TutorialStep.Bounce) return;
        ShowFlash("bounce", TutorialStep.CounterAttack, 2f);
    }

    void OnDamageTaken(int dmg)
    {
        // Skip bounce step if it never triggered
        if (_step > TutorialStep.CounterAttack) return;
        if (_step <= TutorialStep.CounterAttack)
            ShowFlash("counterattack", TutorialStep.Upgrade, 2f);
    }

    void OnUpgradesOffered(UpgradeData[] data)
    {
        // Skip any pending steps — upgrade screen is the final tutorial step
        if (_step >= TutorialStep.Done) return;
        ShowFlash("upgrade", TutorialStep.Done, 2.5f);
    }

    // ─── STEP LOGIC ───

    void AdvanceTo(TutorialStep next)
    {
        StopAllAnimations();
        _step = next;
        if (next == TutorialStep.Done) { CompleteTutorial(); return; }
        ShowStep(next);
    }

    void ShowStep(TutorialStep step)
    {
        StopAllAnimations();
        bool isEN = IsEN();

        switch (step)
        {
            case TutorialStep.Aim:
                _canvas.gameObject.SetActive(true);
                ShowBubble(GetText("aim", isEN));
                StartHandAnimation();
                break;

            case TutorialStep.Shoot:
                HideHand();
                ShowBubble(GetText("shoot", isEN));
                break;

            // Hidden steps — wait for event
            case TutorialStep.Bounce:
            case TutorialStep.CounterAttack:
            case TutorialStep.Upgrade:
                HideAll();
                break;
        }
    }

    void ShowFlash(string textKey, TutorialStep next, float duration)
    {
        StopAllAnimations();
        _canvas.gameObject.SetActive(true);
        ShowBubble(GetText(textKey, IsEN()));
        _autoDismiss = StartCoroutine(AutoDismissAndAdvance(next, duration));
    }

    IEnumerator AutoDismissAndAdvance(TutorialStep next, float delay)
    {
        yield return new WaitForSeconds(delay);
        AdvanceTo(next);
    }

    void CompleteTutorial()
    {
        HideAll();
        _active = false;
        UnsubscribeEvents();
        if (SaveManager.Instance != null) SaveManager.Instance.SetTutorialDone();
        if (_canvas != null) Destroy(_canvas.gameObject);
    }

    // ─── UI CONTROL ───

    void ShowBubble(string message)
    {
        _text.text = message;
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeBubble(1f, 0.2f));
    }

    void HideHand()
    {
        if (_handAnim != null) { StopCoroutine(_handAnim); _handAnim = null; }
        if (_handImage != null) _handImage.gameObject.SetActive(false);
    }

    void HideAll()
    {
        StopAllAnimations();
        if (_bubbleGroup != null) _bubbleGroup.alpha = 0f;
        if (_canvas != null) _canvas.gameObject.SetActive(false);
        HideHand();
    }

    void StopAllAnimations()
    {
        if (_handAnim != null) { StopCoroutine(_handAnim); _handAnim = null; }
        if (_autoDismiss != null) { StopCoroutine(_autoDismiss); _autoDismiss = null; }
        if (_fadeRoutine != null) { StopCoroutine(_fadeRoutine); _fadeRoutine = null; }
    }

    // ─── ANIMATED HAND ───

    void StartHandAnimation()
    {
        _handImage.gameObject.SetActive(true);
        _handAnim = StartCoroutine(SwipeHandLoop());
    }

    IEnumerator SwipeHandLoop()
    {
        // Swipe from top to bottom (pull-down aiming gesture)
        Vector2 startPos = new Vector2(0, -300);
        Vector2 endPos = new Vector2(60, -620);

        while (true)
        {
            // Appear at start position
            _handRect.anchoredPosition = startPos;
            _handImage.color = new Color(1f, 1f, 1f, 0f);

            // Fade in
            for (float t = 0; t < 0.2f; t += Time.deltaTime)
            {
                _handImage.color = new Color(1f, 1f, 1f, (t / 0.2f) * 0.9f);
                yield return null;
            }
            _handImage.color = new Color(1f, 1f, 1f, 0.9f);

            // Brief hold (finger press)
            yield return new WaitForSeconds(0.25f);

            // Swipe upward
            for (float t = 0; t < 0.6f; t += Time.deltaTime)
            {
                float p = t / 0.6f;
                float ease = p * p * (3f - 2f * p); // smoothstep
                _handRect.anchoredPosition = Vector2.Lerp(startPos, endPos, ease);
                yield return null;
            }
            _handRect.anchoredPosition = endPos;

            // Hold at end
            yield return new WaitForSeconds(0.15f);

            // Fade out
            for (float t = 0; t < 0.25f; t += Time.deltaTime)
            {
                float a = 1f - (t / 0.25f);
                _handImage.color = new Color(1f, 1f, 1f, a * 0.9f);
                yield return null;
            }
            _handImage.color = new Color(1f, 1f, 1f, 0f);

            // Pause before loop
            yield return new WaitForSeconds(0.6f);
        }
    }

    // ─── FADE ───

    IEnumerator FadeBubble(float targetAlpha, float duration)
    {
        float start = _bubbleGroup.alpha;
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            _bubbleGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        _bubbleGroup.alpha = targetAlpha;
    }

    // ─── HELPERS ───

    bool IsEN() => SaveManager.Instance != null && SaveManager.Instance.Language == "EN";

    string GetText(string key, bool en)
    {
        switch (key)
        {
            case "aim":
                return en ? "Swipe to aim!" : "Glisse pour viser !";
            case "shoot":
                return en ? "Release to fire!" : "Relache pour tirer !";
            case "bounce":
                return en ? "Bounces off walls!" : "Rebondit sur les murs !";
            case "counterattack":
                return en ? "Monsters fight back!" : "Les monstres contre-attaquent !";
            case "upgrade":
                return en ? "Pick an upgrade!" : "Choisis une amelioration !";
            default:
                return key;
        }
    }
}
