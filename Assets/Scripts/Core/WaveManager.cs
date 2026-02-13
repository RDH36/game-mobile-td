using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    private WaveData[] _fixedWaves;
    private int _currentWaveIndex = -1;
    private EnemySpawner _spawner;
    private ArrowManager _arrowManager;
    private BowHealth _bowHealth;

    private float _nextWaveTimer;
    private bool _waitingForNextWave;

    // EnemyData refs for infinite generation
    private EnemyData _weak;
    private EnemyData _medium;
    private EnemyData _strong;
    private EnemyData _elite;

    // Boss data
    private Dictionary<int, BossData> _bossByWave = new Dictionary<int, BossData>();

    public int CurrentWave => _currentWaveIndex + 1;
    public int FixedWaveCount => _fixedWaves != null ? _fixedWaves.Length : 0;
    public float NextWaveTimer => _nextWaveTimer;
    public bool WaitingForNextWave => _waitingForNextWave;

    public event System.Action<int> OnWaveStarted;
    public event System.Action<int> OnWaveCompleted;
    public event System.Action<BossData> OnBossWaveStarted;

    void Start()
    {
        _spawner = FindFirstObjectByType<EnemySpawner>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _bowHealth = FindFirstObjectByType<BowHealth>();

        _fixedWaves = LoadWaveData();
        LoadEnemyData();
        LoadBossData();

        if (_fixedWaves.Length == 0)
        {
            Debug.LogError("WaveManager: No WaveData found!");
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

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
            StartWave(0);
        }
        else if (state == GameState.WaveComplete)
        {
            OnWaveCompleted?.Invoke(CurrentWave);
            StartCoroutine(NextWaveSequence());
        }
    }

    public bool IsBossWave(int waveNum)
    {
        return _bossByWave.ContainsKey(waveNum);
    }

    public BossData GetBossData(int waveNum)
    {
        _bossByWave.TryGetValue(waveNum, out BossData data);
        return data;
    }

    void StartWave(int index)
    {
        _currentWaveIndex = index;
        int waveNum = index + 1;

        // Check for boss wave
        if (IsBossWave(waveNum))
        {
            BossData bossData = _bossByWave[waveNum];
            WaveEntry[] guards = GenerateBossGuards(waveNum);
            GameManager.Instance?.SetState(GameState.Playing);
            _spawner.SpawnBossWave(bossData, guards);
            OnBossWaveStarted?.Invoke(bossData);
            OnWaveStarted?.Invoke(CurrentWave);
            int guardCount = 0;
            if (guards != null) foreach (var g in guards) guardCount += g.count;
            Debug.Log($"Wave {CurrentWave} started (BOSS): {bossData.bossName} — {bossData.maxHP} HP, pattern: {bossData.pattern}, guards: {guardCount}");
            return;
        }

        if (index < FixedWaveCount)
        {
            // Fixed SO waves (1-4)
            WaveData data = _fixedWaves[index];
            GameManager.Instance?.SetState(GameState.Playing);
            _spawner.SpawnWave(data.entries);
            OnWaveStarted?.Invoke(CurrentWave);
            Debug.Log($"Wave {CurrentWave} started (fixed): {data.waveName} ({data.TotalEnemies} enemies)");
        }
        else
        {
            // Infinite generated waves (5+)
            float hpMult = InfiniteWaveGenerator.GetHPMultiplier(waveNum);
            float coinMult = InfiniteWaveGenerator.GetCoinMultiplier(waveNum);
            WaveEntry[] entries = InfiniteWaveGenerator.Generate(waveNum, _weak, _medium, _strong, _elite);

            GameManager.Instance?.SetState(GameState.Playing);
            _spawner.SpawnWave(entries, hpMult, coinMult);
            OnWaveStarted?.Invoke(CurrentWave);

            int total = 0;
            foreach (var e in entries) total += e.count;
            Debug.Log($"Wave {CurrentWave} started (generated): {total} enemies, HP x{hpMult:F2}, Coins x{coinMult:F2}");
        }
    }

    private IEnumerator NextWaveSequence()
    {
        _waitingForNextWave = true;
        GameManager.Instance?.SetState(GameState.WaveComplete);

        yield return new WaitForSeconds(3f);

        GameManager.Instance?.SetState(GameState.UpgradeSelection);

        if (UpgradeManager.Instance != null)
        {
            while (UpgradeManager.Instance.WaitingForSelection)
                yield return null;
        }

        _waitingForNextWave = false;

        _bowHealth.ResetHP();
        _arrowManager.ResetArrows();
        StartWave(_currentWaveIndex + 1);
    }

    WaveEntry[] GenerateBossGuards(int waveNum)
    {
        // Guard composition scales with boss difficulty
        switch (waveNum)
        {
            case 10: // BlobKing — 4 weak guards
                return new[] { new WaveEntry { enemyData = _weak, count = 4 } };

            case 20: // Speedy — 4 weak + 2 medium
                return new[]
                {
                    new WaveEntry { enemyData = _weak, count = 4 },
                    new WaveEntry { enemyData = _medium, count = 2 }
                };

            case 30: // Guardian — 3 weak + 4 medium
                return new[]
                {
                    new WaveEntry { enemyData = _weak, count = 3 },
                    new WaveEntry { enemyData = _medium, count = 4 }
                };

            case 40: // Splitter — 4 medium + 3 strong
                return new[]
                {
                    new WaveEntry { enemyData = _medium, count = 4 },
                    new WaveEntry { enemyData = _strong, count = 3 }
                };

            case 50: // Overlord — 3 medium + 4 strong + 2 elite
                return new[]
                {
                    new WaveEntry { enemyData = _medium, count = 3 },
                    new WaveEntry { enemyData = _strong, count = 4 },
                    new WaveEntry { enemyData = _elite, count = 2 }
                };

            default: // Any other boss wave — scale guards with wave number
                int guardCount = Mathf.Min(3 + waveNum / 15, 8);
                return new[] { new WaveEntry { enemyData = _medium ?? _weak, count = guardCount } };
        }
    }

    void LoadEnemyData()
    {
        _weak = Resources.Load<EnemyData>("EnemyData/EnemyData_Weak");
        _medium = Resources.Load<EnemyData>("EnemyData/EnemyData_Medium");
        _strong = Resources.Load<EnemyData>("EnemyData/EnemyData_Strong");
        _elite = Resources.Load<EnemyData>("EnemyData/EnemyData_Elite");

        if (_weak == null) Debug.LogWarning("WaveManager: EnemyData_Weak not found in Resources/EnemyData/");
        if (_medium == null) Debug.LogWarning("WaveManager: EnemyData_Medium not found in Resources/EnemyData/");
        if (_strong == null) Debug.LogWarning("WaveManager: EnemyData_Strong not found in Resources/EnemyData/");
        if (_elite == null) Debug.LogWarning("WaveManager: EnemyData_Elite not found in Resources/EnemyData/");
    }

    void LoadBossData()
    {
        BossData[] bosses = Resources.LoadAll<BossData>("BossData");
        foreach (var boss in bosses)
        {
            if (boss.bossWave > 0)
            {
                _bossByWave[boss.bossWave] = boss;
                Debug.Log($"WaveManager: Loaded boss '{boss.bossName}' for wave {boss.bossWave}");
            }
        }
    }

    WaveData[] LoadWaveData()
    {
        WaveData[] loaded = Resources.LoadAll<WaveData>("Waves");
        System.Array.Sort(loaded, (a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
        return loaded;
    }
}
