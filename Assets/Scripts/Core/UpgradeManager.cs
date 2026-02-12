using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    private UpgradeData[] _allUpgrades;
    private UpgradeData[] _currentChoices;
    private bool _waitingForSelection;
    private WaveManager _waveManager;

    public UpgradeData[] CurrentChoices => _currentChoices;
    public bool WaitingForSelection => _waitingForSelection;

    public event System.Action<UpgradeData[]> OnUpgradesOffered;
    public event System.Action<UpgradeData> OnUpgradeSelected;
    public event System.Action OnUpgradePhaseComplete;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _allUpgrades = LoadUpgradeData();
        _waveManager = FindFirstObjectByType<WaveManager>();

        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.UpgradeSelection)
        {
            OfferUpgrades();
        }
    }

    void OfferUpgrades()
    {
        if (_allUpgrades.Length == 0)
        {
            Debug.LogWarning("UpgradeManager: No upgrades found, skipping selection.");
            OnUpgradePhaseComplete?.Invoke();
            return;
        }

        // Pick 3 random upgrades (or fewer if not enough)
        List<UpgradeData> pool = new List<UpgradeData>(_allUpgrades);
        int count = Mathf.Min(3, pool.Count);
        _currentChoices = new UpgradeData[count];

        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            _currentChoices[i] = pool[idx];
            pool.RemoveAt(idx);
        }

        _waitingForSelection = true;
        OnUpgradesOffered?.Invoke(_currentChoices);
        Debug.Log($"UpgradeManager: Offering {count} upgrades. Pick one!");
    }

    public void SelectUpgrade(int choiceIndex)
    {
        if (!_waitingForSelection || _currentChoices == null) return;
        if (choiceIndex < 0 || choiceIndex >= _currentChoices.Length) return;

        UpgradeData chosen = _currentChoices[choiceIndex];

        int scaledCost = GetScaledCost(chosen);
        if (CoinManager.Instance != null && !CoinManager.Instance.SpendCoins(scaledCost))
        {
            Debug.Log($"Not enough coins for {chosen.upgradeName} (cost: {scaledCost})");
            return;
        }

        ApplyUpgrade(chosen);
        _waitingForSelection = false;
        _currentChoices = null;

        OnUpgradeSelected?.Invoke(chosen);
        OnUpgradePhaseComplete?.Invoke();
    }

    public void SkipUpgrade()
    {
        if (!_waitingForSelection) return;

        _waitingForSelection = false;
        _currentChoices = null;

        Debug.Log("Upgrade skipped.");
        OnUpgradePhaseComplete?.Invoke();
    }

    void ApplyUpgrade(UpgradeData data)
    {
        ArrowManager arrowMgr = FindFirstObjectByType<ArrowManager>();
        if (arrowMgr == null) return;

        switch (data.upgradeType)
        {
            case UpgradeType.ArrowDurability:
                arrowMgr.AddBonusDurability(data.value);
                break;
            case UpgradeType.ArrowCount:
                arrowMgr.AddBonusArrowCount(data.value);
                break;
            case UpgradeType.ArrowDamage:
                arrowMgr.AddBonusDamage(data.value);
                break;
        }

        Debug.Log($"Upgrade applied: {data.upgradeName} (+{data.value})");
    }

    public int GetScaledCost(UpgradeData data)
    {
        int wave = _waveManager != null ? _waveManager.CurrentWave : 1;
        return Mathf.CeilToInt(data.cost * GetPriceMultiplier(wave));
    }

    public static float GetPriceMultiplier(int wave)
    {
        if (wave >= 40) return 4f;
        if (wave >= 30) return 3f;
        if (wave >= 20) return 2f;
        if (wave >= 10) return 1.5f;
        return 1f;
    }

    UpgradeData[] LoadUpgradeData()
    {
        UpgradeData[] data = Resources.LoadAll<UpgradeData>("Upgrades");
        Debug.Log($"UpgradeManager: Loaded {data.Length} upgrades from Resources/Upgrades/");
        return data;
    }
}
