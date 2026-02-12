using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class GameOverScreenUI : MonoBehaviour
{
    private GameObject _panel;
    private TextMeshProUGUI _waveText;
    private TextMeshProUGUI _bestWaveText;
    private TextMeshProUGUI _coinsText;
    private TextMeshProUGUI _gemsEarnedText;
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
        _bestWaveText = FindTMP(panelT, "BestWaveStat");
        _coinsText = FindTMP(panelT, "CoinsStat") ?? FindTMP(panelT, "GemsStat");
        _gemsEarnedText = FindTMP(panelT, "GemsEarned");
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
        var coins = CoinManager.Instance;
        int currentWave = wave != null ? wave.CurrentWave : 0;

        // Save high score
        bool isNewBest = false;
        if (SaveManager.Instance != null)
            isNewBest = SaveManager.Instance.TrySetHighScore(currentWave);

        int bestWave = SaveManager.Instance != null ? SaveManager.Instance.HighScore : currentWave;

        // Permanent gems: premium currency, small drops
        int gemsEarned = GetGemsForWave(currentWave);
        if (SaveManager.Instance != null)
            SaveManager.Instance.AddGems(gemsEarned);

        if (_waveText != null)
            _waveText.text = $"Vague atteinte : {currentWave}";
        if (_bestWaveText != null)
            _bestWaveText.text = isNewBest ? $"NOUVEAU RECORD ! Vague {bestWave}" : $"Meilleur : Vague {bestWave}";
        if (_coinsText != null)
            _coinsText.text = $"Coins : {(coins != null ? coins.CoinsThisRun : 0)}";
        if (_gemsEarnedText != null)
            _gemsEarnedText.text = $"Gems gagnes : +{gemsEarned}";
        if (_killsText != null)
            _killsText.text = $"Ennemis tues : {(coins != null ? coins.TotalKillsThisRun : 0)}";
        if (_arrowsText != null)
            _arrowsText.text = $"Tirs effectues : {(coins != null ? coins.TotalArrowsFiredThisRun : 0)}";
    }

    static int GetGemsForWave(int wave)
    {
        if (wave >= 40) return 8;
        if (wave >= 30) return 5;
        if (wave >= 20) return 3;
        if (wave >= 10) return 2;
        return 1;
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
        SFXManager.Instance?.PlayUIClick();
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    void OnMenu()
    {
        SFXManager.Instance?.PlayUIClick();
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}
