using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPanel : MonoBehaviour
{
    private bool _visible;
    private bool _godMode;
    private int _customWave = 10;
    private Vector2 _scrollPos;
    private Rect _windowRect;
    private bool _rectInit;

    private GUISkin _skin;

    private WaveManager _waveManager;
    private EnemySpawner _spawner;
    private ArrowManager _arrowManager;
    private BowHealth _bowHealth;
    private CoinManager _coinManager;

    void Start()
    {
        _waveManager = FindFirstObjectByType<WaveManager>();
        _spawner = FindFirstObjectByType<EnemySpawner>();
        _arrowManager = FindFirstObjectByType<ArrowManager>();
        _bowHealth = FindFirstObjectByType<BowHealth>();
        _coinManager = FindFirstObjectByType<CoinManager>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
            _visible = !_visible;

        if (_godMode && _bowHealth != null && _bowHealth.CurrentHP < _bowHealth.MaxHP)
            _bowHealth.ResetHP();
    }

    void BuildSkin()
    {
        if (_skin != null) return;

        _skin = ScriptableObject.CreateInstance<GUISkin>();
        // Copy from default
        _skin.window = new GUIStyle(GUI.skin.window);
        _skin.button = new GUIStyle(GUI.skin.button);
        _skin.label = new GUIStyle(GUI.skin.label);
        _skin.toggle = new GUIStyle(GUI.skin.toggle);
        _skin.textField = new GUIStyle(GUI.skin.textField);
        _skin.scrollView = new GUIStyle(GUI.skin.scrollView);
        _skin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
        _skin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);

        int fs = Mathf.RoundToInt(Screen.height / 50f); // ~22px at 1080p, ~38px at 1920p
        fs = Mathf.Clamp(fs, 14, 50);

        _skin.window.fontSize = fs;
        _skin.button.fontSize = fs;
        _skin.button.fontStyle = FontStyle.Bold;
        _skin.label.fontSize = fs;
        _skin.toggle.fontSize = fs;
        _skin.textField.fontSize = fs;
        _skin.textField.alignment = TextAnchor.MiddleCenter;
    }

    void OnGUI()
    {
        if (!_visible) return;

        BuildSkin();

        if (!_rectInit)
        {
            float w = Screen.width * 0.35f;
            float h = Screen.height * 0.85f;
            _windowRect = new Rect(Screen.width - w - 8, Screen.height * 0.05f, w, h);
            _rectInit = true;
        }

        var oldSkin = GUI.skin;
        GUI.skin = _skin;
        _windowRect = GUI.Window(9999, _windowRect, DrawWindow, "DEBUG (F1)");
        GUI.skin = oldSkin;
    }

    void DrawWindow(int id)
    {
        float btnH = Screen.height / 22f;
        float bigBtnH = Screen.height / 18f;

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        int wave = _waveManager != null ? _waveManager.CurrentWave : 0;
        string state = GameManager.Instance != null ? GameManager.Instance.CurrentState.ToString() : "?";
        GUILayout.Label($"Wave: {wave} | {state}");
        if (_arrowManager != null)
            GUILayout.Label($"Dmg+{_arrowManager.BonusDamage} Dur+{_arrowManager.BonusDurability} Arr+{_arrowManager.BonusArrowCount}");

        // BOSS
        YellowLabel("BOSS WAVES");
        Row("W10 BlobKing", () => JumpToWave(10), "W20 Speedy", () => JumpToWave(20), bigBtnH);
        Row("W30 Guardian", () => JumpToWave(30), "W40 Splitter", () => JumpToWave(40), bigBtnH);
        if (GUILayout.Button("W50 OVERLORD", GUILayout.Height(bigBtnH))) JumpToWave(50);

        // WAVE
        YellowLabel("CUSTOM WAVE");
        GUILayout.BeginHorizontal();
        string ws = GUILayout.TextField(_customWave.ToString(), GUILayout.Width(Screen.width * 0.08f), GUILayout.Height(btnH));
        int.TryParse(ws, out _customWave);
        if (GUILayout.Button("GO", GUILayout.Height(btnH))) JumpToWave(_customWave);
        GUILayout.EndHorizontal();

        // UPGRADES
        YellowLabel("UPGRADES");
        Row("Dmg+1", () => ApplyUpgrade(UpgradeType.ArrowDamage, 1), "Dmg+5", () => ApplyUpgrade(UpgradeType.ArrowDamage, 5), btnH);
        Row("Dur+1", () => ApplyUpgrade(UpgradeType.ArrowDurability, 1), "Dur+5", () => ApplyUpgrade(UpgradeType.ArrowDurability, 5), btnH);
        Row("Arr+1", () => ApplyUpgrade(UpgradeType.ArrowCount, 1), "Arr+3", () => ApplyUpgrade(UpgradeType.ArrowCount, 3), btnH);
        if (GUILayout.Button("Reset Upgrades", GUILayout.Height(btnH))) ResetUpgrades();

        // ECONOMY
        YellowLabel("ECONOMY");
        Row("+100", () => AddCoins(100), "+1000", () => AddCoins(1000), btnH);

        // CHEATS
        YellowLabel("CHEATS");
        _godMode = GUILayout.Toggle(_godMode, " GOD MODE", GUILayout.Height(btnH));
        Row("Kill All", KillAllEnemies, "Refill Arrows", RefillArrows, btnH);
        if (GUILayout.Button("Heal Bow", GUILayout.Height(btnH))) HealBow();

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    void YellowLabel(string t)
    {
        var s = new GUIStyle(GUI.skin.label) { richText = true };
        GUILayout.Label($"<b><color=yellow>{t}</color></b>", s);
    }

    void Row(string a, System.Action actA, string b, System.Action actB, float h)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(a, GUILayout.Height(h))) actA();
        if (GUILayout.Button(b, GUILayout.Height(h))) actB();
        GUILayout.EndHorizontal();
    }

    // --- Actions ---

    void JumpToWave(int waveNum)
    {
        if (_waveManager == null || _spawner == null) return;
        _spawner.ClearEnemies();
        if (_arrowManager != null) _arrowManager.DestroyAllArrows();
        var m = typeof(WaveManager).GetMethod("StartWave",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (m != null)
        {
            GameManager.Instance?.SetState(GameState.Playing);
            if (_bowHealth != null) _bowHealth.ResetHP();
            if (_arrowManager != null) _arrowManager.ResetArrows();
            m.Invoke(_waveManager, new object[] { waveNum - 1 });
        }
    }

    void ApplyUpgrade(UpgradeType type, int amount)
    {
        if (_arrowManager == null) return;
        switch (type)
        {
            case UpgradeType.ArrowDamage: _arrowManager.AddBonusDamage(amount); break;
            case UpgradeType.ArrowDurability: _arrowManager.AddBonusDurability(amount); break;
            case UpgradeType.ArrowCount:
                _arrowManager.AddBonusArrowCount(amount);
                _arrowManager.ResetArrows();
                break;
        }
    }

    void ResetUpgrades() { if (_arrowManager != null) { _arrowManager.ResetUpgrades(); _arrowManager.ResetArrows(); } }
    void AddCoins(int a) { if (_coinManager != null) _coinManager.AddCoins(a); }
    void KillAllEnemies()
    {
        if (_spawner == null) return;
        var list = new System.Collections.Generic.List<Enemy>(_spawner.ActiveEnemies);
        foreach (var e in list)
            if (e != null && e.Health != null && !e.Health.IsDead) e.Health.TakeDamage(9999);
    }
    void RefillArrows() { if (_arrowManager != null) _arrowManager.ResetArrows(); }
    void HealBow() { if (_bowHealth != null) _bowHealth.ResetHP(); }
}
