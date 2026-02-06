using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryScreenUI : MonoBehaviour
{
    private GameObject _panel;
    private TextMeshProUGUI _gemsText;
    private TextMeshProUGUI _killsText;
    private TextMeshProUGUI _arrowsText;
    private Button _replayButton;
    private Button _menuButton;

    void Awake()
    {
        FindUIElements();
        if (_panel != null) _panel.SetActive(false);
    }

    void Start()
    {
        if (_replayButton != null)
            _replayButton.onClick.AddListener(OnReplay);
        if (_menuButton != null)
            _menuButton.onClick.AddListener(OnMenu);

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void FindUIElements()
    {
        Transform canvas = transform;
        Transform panelT = canvas.Find("VictoryPanel");
        if (panelT == null) return;

        _panel = panelT.gameObject;
        _gemsText = FindTMP(panelT, "GemsStat");
        _killsText = FindTMP(panelT, "KillsStat");
        _arrowsText = FindTMP(panelT, "ArrowsStat");

        Transform replayT = panelT.Find("ReplayButton");
        if (replayT != null) _replayButton = replayT.GetComponent<Button>();
        Transform menuT = panelT.Find("MenuButton");
        if (menuT != null) _menuButton = menuT.GetComponent<Button>();
    }

    TextMeshProUGUI FindTMP(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        return t != null ? t.GetComponent<TextMeshProUGUI>() : null;
    }

    void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Victory)
            Show();
        else if (_panel != null)
            _panel.SetActive(false);
    }

    void Show()
    {
        if (_panel == null) return;
        _panel.SetActive(true);

        var gems = GemManager.Instance;

        if (_gemsText != null)
            _gemsText.text = $"Gemmes : {(gems != null ? gems.GemsThisRun : 0)}";
        if (_killsText != null)
            _killsText.text = $"Ennemis tues : {(gems != null ? gems.TotalKillsThisRun : 0)}";
        if (_arrowsText != null)
            _arrowsText.text = $"Fleches tirees : {(gems != null ? gems.TotalArrowsFiredThisRun : 0)}";
    }

    void OnReplay()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    void OnMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}
