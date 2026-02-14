using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    WaveComplete,
    UpgradeSelection,
    GameOver,
    Victory
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Menu;

    public event System.Action<GameState> OnGameStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // Fire initial state so all listeners can react to Menu
        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        // Launch tutorial BEFORE state change so it can receive the Playing event
        if (SaveManager.Instance != null && !SaveManager.Instance.TutorialDone)
        {
            if (TutorialManager.Instance == null)
                gameObject.AddComponent<TutorialManager>();
        }

        SetState(GameState.Playing);
    }
}
