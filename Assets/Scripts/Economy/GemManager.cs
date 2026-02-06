using UnityEngine;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance { get; private set; }

    private int _gemsThisRun;
    private int _gemsThisWave;
    private int _killsThisArrow;
    private int _damageTakenThisWave;
    private int _totalKillsThisRun;
    private int _totalArrowsFiredThisRun;

    public int GemsThisRun => _gemsThisRun;
    public int GemsThisWave => _gemsThisWave;
    public int KillsThisArrow => _killsThisArrow;
    public bool PerfectWave => _damageTakenThisWave == 0;
    public int TotalKillsThisRun => _totalKillsThisRun;
    public int TotalArrowsFiredThisRun => _totalArrowsFiredThisRun;

    public event System.Action<int> OnGemsChanged; // total run gems

    private EnemySpawner _spawner;
    private ArrowManager _arrowManager;
    private BowHealth _bowHealth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _spawner = FindFirstObjectByType<EnemySpawner>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _bowHealth = FindFirstObjectByType<BowHealth>();

        if (_spawner != null)
            _spawner.OnEnemyKilled += HandleEnemyKilled;
        if (_arrowManager != null)
        {
            _arrowManager.OnArrowLanded += HandleArrowLanded;
            _arrowManager.OnArrowCountChanged += HandleArrowFired;
        }
        if (_bowHealth != null)
            _bowHealth.OnDamageTaken += HandleDamageTaken;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        if (_spawner != null)
            _spawner.OnEnemyKilled -= HandleEnemyKilled;
        if (_arrowManager != null)
        {
            _arrowManager.OnArrowLanded -= HandleArrowLanded;
            _arrowManager.OnArrowCountChanged -= HandleArrowFired;
        }
        if (_bowHealth != null)
            _bowHealth.OnDamageTaken -= HandleDamageTaken;
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleEnemyKilled(Enemy enemy)
    {
        int gems = enemy.GetGemDrop();
        _gemsThisRun += gems;
        _gemsThisWave += gems;
        _killsThisArrow++;
        _totalKillsThisRun++;
        OnGemsChanged?.Invoke(_gemsThisRun);
    }

    void HandleArrowLanded()
    {
        // Combo bonus: 3+ kills with 1 arrow = +5 gems per kill beyond 2
        if (_killsThisArrow >= 3)
        {
            int comboBonus = (_killsThisArrow - 2) * 5;
            _gemsThisRun += comboBonus;
            _gemsThisWave += comboBonus;
            OnGemsChanged?.Invoke(_gemsThisRun);
            Debug.Log($"COMBO x{_killsThisArrow}! +{comboBonus} bonus gems");
        }
        _killsThisArrow = 0;
    }

    void HandleArrowFired(int remaining)
    {
        _totalArrowsFiredThisRun++;
    }

    void HandleDamageTaken(int damage)
    {
        _damageTakenThisWave += damage;
    }

    void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.WaveComplete)
        {
            // Perfect wave bonus: +20% gems if 0 damage taken
            if (_damageTakenThisWave == 0 && _gemsThisWave > 0)
            {
                int perfectBonus = Mathf.CeilToInt(_gemsThisWave * 0.2f);
                _gemsThisRun += perfectBonus;
                _gemsThisWave += perfectBonus;
                OnGemsChanged?.Invoke(_gemsThisRun);
                Debug.Log($"PERFECT WAVE! +{perfectBonus} bonus gems (20%)");
            }
        }
        else if (state == GameState.Playing)
        {
            // New wave starting: reset wave tracking
            _gemsThisWave = 0;
            _damageTakenThisWave = 0;
            _killsThisArrow = 0;
        }
    }

    public bool SpendGems(int amount)
    {
        if (_gemsThisRun < amount) return false;
        _gemsThisRun -= amount;
        OnGemsChanged?.Invoke(_gemsThisRun);
        return true;
    }

    public void ResetRun()
    {
        _gemsThisRun = 0;
        _gemsThisWave = 0;
        _killsThisArrow = 0;
        _damageTakenThisWave = 0;
        _totalKillsThisRun = 0;
        _totalArrowsFiredThisRun = 0;
        OnGemsChanged?.Invoke(_gemsThisRun);
    }
}
