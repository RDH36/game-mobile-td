using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    private GameObject _panel;
    private Button _playButton;

    void Awake()
    {
        FindUIElements();
    }

    void Start()
    {
        if (_playButton != null)
            _playButton.onClick.AddListener(OnPlay);

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        // Show menu on start if state is Menu
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Menu)
        {
            if (_panel != null) _panel.SetActive(true);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void FindUIElements()
    {
        Transform canvas = transform;
        Transform panelT = canvas.Find("MainMenuPanel");
        if (panelT == null) return;

        _panel = panelT.gameObject;
        Transform playT = panelT.Find("PlayButton");
        if (playT != null) _playButton = playT.GetComponent<Button>();
    }

    void HandleGameStateChanged(GameState state)
    {
        if (_panel == null) return;
        _panel.SetActive(state == GameState.Menu);
    }

    void OnPlay()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }
}
