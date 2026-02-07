using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class GameOverScreenUI : MonoBehaviour
{
    private GameObject _panel;
    private TextMeshProUGUI _waveText;
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
        Transform panelT = canvas.Find("GameOverPanel");
        if (panelT == null) return;

        _panel = panelT.gameObject;
        _waveText = FindTMP(panelT, "WaveStat");
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
        if (state == GameState.GameOver)
            Show();
        else if (_panel != null)
            _panel.SetActive(false);
    }

    void Show()
    {
        if (_panel == null) return;
        _panel.SetActive(true);
        SpringInChildren(_panel.transform);

        var wave = FindFirstObjectByType<WaveManager>();
        var gems = GemManager.Instance;

        if (_waveText != null)
            _waveText.text = $"Vague atteinte : {(wave != null ? wave.CurrentWave : 0)}";
        if (_gemsText != null)
            _gemsText.text = $"Gemmes : {(gems != null ? gems.GemsThisRun : 0)}";
        if (_killsText != null)
            _killsText.text = $"Ennemis tues : {(gems != null ? gems.TotalKillsThisRun : 0)}";
        if (_arrowsText != null)
            _arrowsText.text = $"Tirs effectues : {(gems != null ? gems.TotalArrowsFiredThisRun : 0)}";
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
