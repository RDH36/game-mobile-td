using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class HUDManager : MonoBehaviour
{
    private Image _hpFill;
    private TextMeshProUGUI _hpText;
    private TextMeshProUGUI _gemText;
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
    private MMSpringScale _gemSpring;
    private MMSpringScale _arrowSpring;
    private MMSpringScale _waveSpring;
    private MMSpringScale _bannerSpring;

    private BowHealth _bowHealth;
    private ArrowManager _arrowManager;
    private WaveManager _waveManager;
    private GemManager _gemManager;

    void Awake()
    {
        FindUIElements();
        if (_bannerObj != null) _bannerObj.SetActive(false);
    }

    void Start()
    {
        _bowHealth = FindFirstObjectByType<BowHealth>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _waveManager = FindFirstObjectByType<WaveManager>();
        _gemManager = FindFirstObjectByType<GemManager>();

        if (_bowHealth != null) _bowHealth.OnHPChanged += UpdateHP;
        if (_arrowManager != null) _arrowManager.OnArrowCountChanged += UpdateArrows;
        if (_waveManager != null) _waveManager.OnWaveStarted += UpdateWave;
        if (_gemManager != null) _gemManager.OnGemsChanged += UpdateGems;

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeSelected += OnUpgradeApplied;

        // Initial values
        if (_bowHealth != null) UpdateHP(_bowHealth.CurrentHP, _bowHealth.MaxHP);
        if (_arrowManager != null) UpdateArrows(_arrowManager.ArrowsRemaining);
        if (_waveManager != null) UpdateWave(_waveManager.CurrentWave, _waveManager.TotalWaves);
        UpdateGems(0);
    }

    void OnDestroy()
    {
        if (_bowHealth != null) _bowHealth.OnHPChanged -= UpdateHP;
        if (_arrowManager != null) _arrowManager.OnArrowCountChanged -= UpdateArrows;
        if (_waveManager != null) _waveManager.OnWaveStarted -= UpdateWave;
        if (_gemManager != null) _gemManager.OnGemsChanged -= UpdateGems;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradeSelected -= OnUpgradeApplied;
    }

    void HandleGameStateChanged(GameState state)
    {
        // Show/hide HUD based on state
        bool showHUD = state == GameState.Playing || state == GameState.WaveComplete;
        if (_hudContent != null) _hudContent.SetActive(showHUD);

        // Banner for wave complete
        if (_bannerObj != null)
        {
            if (state == GameState.WaveComplete)
            {
                int waveNum = _waveManager != null ? _waveManager.CurrentWave : 0;
                if (_bannerText != null) _bannerText.text = $"VAGUE {waveNum} TERMINEE !";
                _bannerObj.SetActive(true);

                // Spring pop-in on banner text (not bg)
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

        // HUD content wrapper (to hide all HUD at once)
        Transform hudContentT = canvas.Find("HUDContent");
        if (hudContentT != null) _hudContent = hudContentT.gameObject;

        // HUD elements are children of HUDContent
        Transform hud = hudContentT != null ? hudContentT : canvas;

        Transform hpBarBg = hud.Find("HPBarBg");
        if (hpBarBg != null)
        {
            Transform hpFillT = hpBarBg.Find("HPFill");
            if (hpFillT != null) _hpFill = hpFillT.GetComponent<Image>();

            Transform hpTextT = hpBarBg.Find("HPText");
            if (hpTextT != null) _hpText = hpTextT.GetComponent<TextMeshProUGUI>();
        }

        Transform gemRow = hud.Find("GemRow");
        if (gemRow != null)
        {
            Transform gemTextT = gemRow.Find("GemText");
            if (gemTextT != null) _gemText = gemTextT.GetComponent<TextMeshProUGUI>();
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

        // Banner and PauseButton are direct children of canvas (not inside HUDContent)
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

    void UpdateWave(int current, int total)
    {
        if (_waveText == null) return;
        _waveText.text = $"VAGUE {current}/{total}";
        BumpSpring(ref _waveSpring, _waveText.transform.parent);
    }

    void UpdateGems(int total)
    {
        if (_gemText == null) return;
        _gemText.text = $"{total}";
        BumpSpring(ref _gemSpring, _gemText.transform.parent);
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
