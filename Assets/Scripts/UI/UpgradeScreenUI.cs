using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class UpgradeScreenUI : MonoBehaviour
{
    private GameObject _panel;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _gemsInfoText;
    private Button[] _upgradeButtons = new Button[3];
    private TextMeshProUGUI[] _upgradeTitles = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] _upgradeCosts = new TextMeshProUGUI[3];
    private Button _skipButton;

    private UpgradeManager _upgradeManager;
    private GemManager _gemManager;
    private WaveManager _waveManager;

    void Awake()
    {
        FindUIElements();
        if (_panel != null) _panel.SetActive(false);
    }

    void Start()
    {
        _upgradeManager = FindFirstObjectByType<UpgradeManager>();
        _gemManager = FindFirstObjectByType<GemManager>();
        _waveManager = FindFirstObjectByType<WaveManager>();

        if (_skipButton != null)
            _skipButton.onClick.AddListener(OnSkipClicked);

        // Listen to OnUpgradesOffered so data is guaranteed ready
        if (_upgradeManager != null)
            _upgradeManager.OnUpgradesOffered += OnUpgradesOffered;

        // Hide on state changes away from UpgradeSelection
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        if (_upgradeManager != null)
            _upgradeManager.OnUpgradesOffered -= OnUpgradesOffered;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void FindUIElements()
    {
        Transform canvas = transform;

        Transform panelT = canvas.Find("UpgradePanel");
        if (panelT != null)
        {
            _panel = panelT.gameObject;

            Transform titleT = panelT.Find("Title");
            if (titleT != null) _titleText = titleT.GetComponent<TextMeshProUGUI>();

            Transform gemsT = panelT.Find("GemsInfo");
            if (gemsT != null) _gemsInfoText = gemsT.GetComponent<TextMeshProUGUI>();

            for (int i = 0; i < 3; i++)
            {
                Transform btnT = panelT.Find($"UpgradeBtn{i}");
                if (btnT != null)
                {
                    _upgradeButtons[i] = btnT.GetComponent<Button>();

                    Transform btnTitle = btnT.Find("Title");
                    if (btnTitle != null) _upgradeTitles[i] = btnTitle.GetComponent<TextMeshProUGUI>();

                    Transform btnCost = btnT.Find("Cost");
                    if (btnCost != null) _upgradeCosts[i] = btnCost.GetComponent<TextMeshProUGUI>();
                }
            }

            Transform skipT = panelT.Find("SkipButton");
            if (skipT != null) _skipButton = skipT.GetComponent<Button>();
        }
    }

    void HandleGameStateChanged(GameState state)
    {
        // Only hide when leaving UpgradeSelection
        if (state != GameState.UpgradeSelection)
            Hide();
    }

    void OnUpgradesOffered(UpgradeData[] choices)
    {
        Show(choices);
    }

    void Show(UpgradeData[] choices)
    {
        if (_panel == null) return;
        _panel.SetActive(true);
        SpringInChildren(_panel.transform);

        int waveNum = _waveManager != null ? _waveManager.CurrentWave : 0;
        if (_titleText != null) _titleText.text = $"VAGUE {waveNum} TERMINEE !";

        int waveGems = _gemManager != null ? _gemManager.GemsThisWave : 0;
        int totalGems = _gemManager != null ? _gemManager.GemsThisRun : 0;
        bool perfect = _gemManager != null && _gemManager.PerfectWave;
        string perfectTxt = perfect ? "  PARFAIT!" : "";
        if (_gemsInfoText != null) _gemsInfoText.text = $"Gemmes: +{waveGems}  (Total: {totalGems}){perfectTxt}";

        if (choices != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_upgradeButtons[i] == null) continue;

                if (i < choices.Length)
                {
                    _upgradeButtons[i].gameObject.SetActive(true);
                    UpgradeData data = choices[i];
                    bool canAfford = totalGems >= data.cost;

                    if (_upgradeTitles[i] != null)
                        _upgradeTitles[i].text = $"{data.upgradeName}\n<size=18>{data.description}</size>";
                    if (_upgradeCosts[i] != null)
                    {
                        _upgradeCosts[i].text = $"{data.cost}";
                        _upgradeCosts[i].color = canAfford ? new Color(1f, 0.85f, 0.2f) : new Color(0.6f, 0.3f, 0.3f);
                    }

                    _upgradeButtons[i].interactable = canAfford;

                    int idx = i;
                    _upgradeButtons[i].onClick.RemoveAllListeners();
                    _upgradeButtons[i].onClick.AddListener(() => OnUpgradeClicked(idx));
                }
                else
                {
                    _upgradeButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void Hide()
    {
        if (_panel != null) _panel.SetActive(false);
    }

    void SpringInChildren(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            var spring = child.GetComponent<MMSpringScale>();
            if (spring == null)
                spring = child.gameObject.AddComponent<MMSpringScale>();
            spring.MoveToInstant(Vector3.zero);
            spring.MoveTo(Vector3.one);
        }
    }

    void OnUpgradeClicked(int index)
    {
        if (_upgradeManager != null)
            _upgradeManager.SelectUpgrade(index);
    }

    void OnSkipClicked()
    {
        if (_upgradeManager != null)
            _upgradeManager.SkipUpgrade();
    }
}
