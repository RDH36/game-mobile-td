using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    public static Vector3 CoinCounterWorldPos { get; private set; }

    private Image _hpFill;
    private TextMeshProUGUI _hpText;
    private TextMeshProUGUI _coinText;
    private TextMeshProUGUI _waveText;
    private TextMeshProUGUI _arrowText;
    private GameObject _bannerObj;
    private TextMeshProUGUI _bannerText;
    private TextMeshProUGUI _buffDurability;
    private TextMeshProUGUI _buffArrows;
    private TextMeshProUGUI _buffDamage;
    private Button _pauseButton;
    private GameObject _hudContent;

    // Feel springs for HUD juice
    private MMSpringScale _coinSpring;
    private MMSpringScale _arrowSpring;
    private MMSpringScale _waveSpring;
    private MMSpringScale _bannerSpring;

    private BowHealth _bowHealth;
    private ArrowManager _arrowManager;
    private WaveManager _waveManager;
    private CoinManager _coinManager;

    void Awake()
    {
        Instance = this;
        FindUIElements();
        if (_bannerObj != null) _bannerObj.SetActive(false);
    }

    void Start()
    {
        _bowHealth = FindFirstObjectByType<BowHealth>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _waveManager = FindFirstObjectByType<WaveManager>();
        _coinManager = FindFirstObjectByType<CoinManager>();

        if (_bowHealth != null) _bowHealth.OnHPChanged += UpdateHP;
        if (_arrowManager != null) _arrowManager.OnArrowCountChanged += UpdateArrows;
        if (_waveManager != null) _waveManager.OnWaveStarted += OnWaveStarted;
        if (_waveManager != null) _waveManager.OnBossWaveStarted += OnBossWaveStarted;
        if (_coinManager != null) _coinManager.OnCoinsChanged += UpdateCoins;

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeSelected += OnUpgradeApplied;

        // Initial values
        if (_bowHealth != null) UpdateHP(_bowHealth.CurrentHP, _bowHealth.MaxHP);
        if (_arrowManager != null) UpdateArrows(_arrowManager.ArrowsRemaining);
        if (_waveManager != null) OnWaveStarted(_waveManager.CurrentWave);
        UpdateCoins(0);
    }

    void LateUpdate()
    {
        // Update coin counter world position for flying coins
        if (_coinText != null && Camera.main != null)
        {
            Canvas canvas = GetComponent<Canvas>();
            Camera uiCam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera : null;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCam, _coinText.transform.position);
            float camDist = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camDist));
            CoinCounterWorldPos = new Vector3(worldPos.x, worldPos.y, 0f);
        }
    }

    public static void BumpCoinCounter()
    {
        if (Instance == null) return;
        Instance.BumpSpring(ref Instance._coinSpring, Instance._coinText != null ? Instance._coinText.transform.parent : null);
    }

    void OnDestroy()
    {
        if (_bowHealth != null) _bowHealth.OnHPChanged -= UpdateHP;
        if (_arrowManager != null) _arrowManager.OnArrowCountChanged -= UpdateArrows;
        if (_waveManager != null) _waveManager.OnWaveStarted -= OnWaveStarted;
        if (_waveManager != null) _waveManager.OnBossWaveStarted -= OnBossWaveStarted;
        if (_coinManager != null) _coinManager.OnCoinsChanged -= UpdateCoins;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeSelected -= OnUpgradeApplied;
    }

    void HandleGameStateChanged(GameState state)
    {
        bool showHUD = state == GameState.Playing || state == GameState.WaveComplete;
        if (_hudContent != null) _hudContent.SetActive(showHUD);

        if (_bannerObj != null)
        {
            if (state == GameState.WaveComplete)
            {
                int waveNum = _waveManager != null ? _waveManager.CurrentWave : 0;
                if (_bannerText != null) _bannerText.text = $"VAGUE {waveNum} TERMINEE !";
                _bannerObj.SetActive(true);

                if (_bannerText != null)
                {
                    if (_bannerSpring == null)
                        _bannerSpring = _bannerText.gameObject.AddComponent<MMSpringScale>();
                    _bannerSpring.MoveToInstant(Vector3.zero);
                    _bannerSpring.MoveTo(Vector3.one);
                }
            }
            else
            {
                _bannerObj.SetActive(false);
            }
        }
    }

    void FindUIElements()
    {
        Transform canvas = transform;

        Transform hudContentT = canvas.Find("HUDContent");
        if (hudContentT != null) _hudContent = hudContentT.gameObject;

        Transform hud = hudContentT != null ? hudContentT : canvas;

        Transform hpBarBg = hud.Find("HPBarBg");
        if (hpBarBg != null)
        {
            Transform hpFillT = hpBarBg.Find("HPFill");
            if (hpFillT != null) _hpFill = hpFillT.GetComponent<Image>();

            Transform hpTextT = hpBarBg.Find("HPText");
            if (hpTextT != null) _hpText = hpTextT.GetComponent<TextMeshProUGUI>();
        }

        // Support both old name (GemRow) and new name (CoinRow)
        Transform coinRow = hud.Find("CoinRow") ?? hud.Find("GemRow");
        if (coinRow != null)
        {
            Transform coinTextT = coinRow.Find("CoinText") ?? coinRow.Find("GemText");
            if (coinTextT != null) _coinText = coinTextT.GetComponent<TextMeshProUGUI>();
        }

        Transform waveBg = hud.Find("WaveBg");
        if (waveBg != null)
        {
            Transform waveTextT = waveBg.Find("WaveText");
            if (waveTextT != null) _waveText = waveTextT.GetComponent<TextMeshProUGUI>();
        }

        Transform arrowBg = hud.Find("ArrowBg");
        if (arrowBg != null)
        {
            Transform arrowTextT = arrowBg.Find("ArrowText");
            if (arrowTextT != null) _arrowText = arrowTextT.GetComponent<TextMeshProUGUI>();
        }

        Transform buffRow = hud.Find("BuffRow");
        if (buffRow != null)
        {
            Transform bd = buffRow.Find("BuffDurability");
            if (bd != null) _buffDurability = bd.GetComponent<TextMeshProUGUI>();
            Transform ba = buffRow.Find("BuffArrows");
            if (ba != null) _buffArrows = ba.GetComponent<TextMeshProUGUI>();
            Transform bdmg = buffRow.Find("BuffDamage");
            if (bdmg != null) _buffDamage = bdmg.GetComponent<TextMeshProUGUI>();
        }

        Transform bannerT = canvas.Find("WaveCompleteBanner");
        if (bannerT != null)
        {
            _bannerObj = bannerT.gameObject;
            Transform bannerTextT = bannerT.Find("BannerText");
            if (bannerTextT != null) _bannerText = bannerTextT.GetComponent<TextMeshProUGUI>();
        }

        Transform pauseT = canvas.Find("PauseButton");
        if (pauseT != null)
        {
            _pauseButton = pauseT.GetComponent<Button>();
            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(OnPauseClicked);
        }
    }

    void OnPauseClicked()
    {
        SFXManager.Instance?.PlayUIClick();
        var pauseUI = FindFirstObjectByType<PauseScreenUI>();
        if (pauseUI != null) pauseUI.TogglePause();
    }

    void UpdateHP(int current, int max)
    {
        if (_hpFill == null || _hpText == null) return;
        _hpFill.fillAmount = (float)current / max;
        _hpText.text = $"{current}/{max}";
    }

    void UpdateArrows(int remaining)
    {
        if (_arrowText == null) return;
        _arrowText.text = $"x{remaining}";
        _arrowText.color = remaining > 0 ? Color.white : new Color(0.9f, 0.3f, 0.3f);
        BumpSpring(ref _arrowSpring, _arrowText.transform.parent);
    }

    void OnWaveStarted(int current)
    {
        if (_waveText == null) return;
        _waveText.text = $"VAGUE {current}";
        BumpSpring(ref _waveSpring, _waveText.transform.parent);
    }

    void OnBossWaveStarted(BossData bossData)
    {
        if (_bannerObj == null || _bannerText == null) return;

        _bannerText.text = $"BOSS !\n{bossData.bossName}";
        _bannerText.color = new Color(1f, 0.3f, 0.3f);
        _bannerObj.SetActive(true);

        if (_bannerSpring == null)
            _bannerSpring = _bannerText.gameObject.AddComponent<MMSpringScale>();
        _bannerSpring.MoveToInstant(Vector3.zero);
        _bannerSpring.MoveTo(Vector3.one);

        // Auto-hide after 2 seconds
        StartCoroutine(HideBannerAfter(2f));
    }

    System.Collections.IEnumerator HideBannerAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_bannerObj != null) _bannerObj.SetActive(false);
        if (_bannerText != null) _bannerText.color = Color.white;
    }

    void UpdateCoins(int total)
    {
        if (_coinText == null) return;
        _coinText.text = $"{total}";
        BumpSpring(ref _coinSpring, _coinText.transform.parent);
    }

    void BumpSpring(ref MMSpringScale spring, Transform target)
    {
        if (target == null) return;
        if (spring == null)
            spring = target.gameObject.AddComponent<MMSpringScale>();
        spring.Bump(new Vector3(0.12f, 0.12f, 0f));
    }

    void OnUpgradeApplied(UpgradeData data)
    {
        UpdateBuffs();
    }

    void UpdateBuffs()
    {
        if (_arrowManager == null) return;

        int dur = _arrowManager.BonusDurability;
        int arr = _arrowManager.BonusArrowCount;
        int dmg = _arrowManager.BonusDamage;

        if (_buffDurability != null)
            _buffDurability.text = dur > 0 ? $"PV +{dur}" : "";
        if (_buffArrows != null)
            _buffArrows.text = arr > 0 ? $"Tir +{arr}" : "";
        if (_buffDamage != null)
            _buffDamage.text = dmg > 0 ? $"Degat +{dmg}" : "";
    }
}
