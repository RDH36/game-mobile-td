using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    private int _coinsThisRun;
    private int _coinsThisWave;
    private int _killsThisArrow;
    private int _damageTakenThisWave;
    private int _totalKillsThisRun;
    private int _totalArrowsFiredThisRun;

    public int CoinsThisRun => _coinsThisRun;
    public int CoinsThisWave => _coinsThisWave;
    public int KillsThisArrow => _killsThisArrow;
    public bool PerfectWave => _damageTakenThisWave == 0;
    public int TotalKillsThisRun => _totalKillsThisRun;
    public int TotalArrowsFiredThisRun => _totalArrowsFiredThisRun;

    public event System.Action<int> OnCoinsChanged; // total run coins

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
        int coins = enemy.GetCoinDrop();
        _coinsThisRun += coins;
        _coinsThisWave += coins;
        _killsThisArrow++;
        _totalKillsThisRun++;
        OnCoinsChanged?.Invoke(_coinsThisRun);

        if (coins > 0 && enemy != null)
            CoinCollectFX.Create(enemy.transform.position, coins);
    }

    void HandleArrowLanded()
    {
        // Combo bonus: 3+ kills with 1 arrow = +5 coins per kill beyond 2
        if (_killsThisArrow >= 3)
        {
            int comboBonus = (_killsThisArrow - 2) * 5;
            _coinsThisRun += comboBonus;
            _coinsThisWave += comboBonus;
            OnCoinsChanged?.Invoke(_coinsThisRun);

            ComboPopup.Create(_killsThisArrow, comboBonus);
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
            // Perfect wave bonus: +20% coins if 0 damage taken
            if (_damageTakenThisWave == 0 && _coinsThisWave > 0)
            {
                int perfectBonus = Mathf.CeilToInt(_coinsThisWave * 0.2f);
                _coinsThisRun += perfectBonus;
                _coinsThisWave += perfectBonus;
                OnCoinsChanged?.Invoke(_coinsThisRun);
                Debug.Log($"PERFECT WAVE! +{perfectBonus} bonus coins (20%)");
            }
        }
        else if (state == GameState.Playing)
        {
            _coinsThisWave = 0;
            _damageTakenThisWave = 0;
            _killsThisArrow = 0;
        }
    }

    public void AddCoins(int amount)
    {
        _coinsThisRun += amount;
        OnCoinsChanged?.Invoke(_coinsThisRun);
    }

    public bool SpendCoins(int amount)
    {
        if (_coinsThisRun < amount) return false;
        _coinsThisRun -= amount;
        OnCoinsChanged?.Invoke(_coinsThisRun);
        return true;
    }

    public void ResetRun()
    {
        _coinsThisRun = 0;
        _coinsThisWave = 0;
        _killsThisArrow = 0;
        _damageTakenThisWave = 0;
        _totalKillsThisRun = 0;
        _totalArrowsFiredThisRun = 0;
        OnCoinsChanged?.Invoke(_coinsThisRun);
    }
}
