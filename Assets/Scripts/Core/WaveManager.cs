using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private float delayBetweenWaves = 2f;

    private WaveData[] _waves;
    private int _currentWaveIndex = -1;
    private EnemySpawner _spawner;
    private ArrowManager _arrowManager;
    private BowHealth _bowHealth;

    private float _nextWaveTimer;
    private bool _waitingForNextWave;

    public int CurrentWave => _currentWaveIndex + 1;
    public int TotalWaves => _waves != null ? _waves.Length : 0;
    public bool IsLastWave => _currentWaveIndex >= TotalWaves - 1;
    public float NextWaveTimer => _nextWaveTimer;
    public bool WaitingForNextWave => _waitingForNextWave;

    public event System.Action<int, int> OnWaveStarted;
    public event System.Action<int> OnWaveCompleted;

    void Start()
    {
        _spawner = FindFirstObjectByType<EnemySpawner>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _bowHealth = FindFirstObjectByType<BowHealth>();

        _waves = LoadWaveData();

        if (_waves.Length == 0)
        {
            Debug.LogError("WaveManager: No WaveData found!");
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

        // Don't auto-start if in Menu state; wait for Playing
        if (GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing)
            StartWave(0);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing && _currentWaveIndex < 0)
        {
            // First time Playing — start wave 1
            StartWave(0);
        }
        else if (state == GameState.WaveComplete)
        {
            OnWaveCompleted?.Invoke(CurrentWave);

            if (IsLastWave)
                StartCoroutine(VictorySequence());
            else
                StartCoroutine(NextWaveSequence());
        }
    }

    void StartWave(int index)
    {
        _currentWaveIndex = index;
        WaveData data = _waves[index];

        GameManager.Instance?.SetState(GameState.Playing);
        _spawner.SpawnWave(data.entries);
        OnWaveStarted?.Invoke(CurrentWave, TotalWaves);

        Debug.Log($"Wave {CurrentWave}/{TotalWaves} started: {data.waveName} ({data.TotalEnemies} enemies)");
    }

    private IEnumerator NextWaveSequence()
    {
        // Show "Wave Complete" state for a moment
        _waitingForNextWave = true;
        GameManager.Instance?.SetState(GameState.WaveComplete);

        // Wait 3 seconds so player sees "VAGUE TERMINEE"
        yield return new WaitForSeconds(3f);

        // Show upgrade selection and wait for player to pick
        GameManager.Instance?.SetState(GameState.UpgradeSelection);

        if (UpgradeManager.Instance != null)
        {
            while (UpgradeManager.Instance.WaitingForSelection)
                yield return null;
        }

        _waitingForNextWave = false;

        // Restore and start next wave
        _bowHealth.ResetHP();
        _arrowManager.ResetArrows();
        StartWave(_currentWaveIndex + 1);
    }

    private IEnumerator VictorySequence()
    {
        Debug.Log("All waves complete! Victory!");
        yield return new WaitForSeconds(1f);
        GameManager.Instance?.SetState(GameState.Victory);
    }

    WaveData[] LoadWaveData()
    {
        WaveData[] loaded = Resources.LoadAll<WaveData>("Waves");
        System.Array.Sort(loaded, (a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
        return loaded;
    }
}
