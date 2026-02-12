using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// In-game debug console. Toggle with F1.
/// Type commands and press Enter to execute.
///
/// Commands:
///   wave 10       — jump to wave 10
///   boss 1-5      — jump to boss (1=W10, 2=W20, 3=W30, 4=W40, 5=W50)
///   dmg 5         — add +5 damage
///   dur 3         — add +3 durability
///   arr 2         — add +2 arrows
///   coins 1000    — add coins
///   god           — toggle god mode
///   kill          — kill all enemies
///   heal          — heal bow to full
///   arrows        — refill arrows
///   reset         — reset all upgrades
///   help          — show commands
/// </summary>
public class DebugConsole : MonoBehaviour
{
    private GameObject _consoleRoot;
    private TMP_InputField _inputField;
    private TextMeshProUGUI _outputText;
    private ScrollRect _scrollRect;
    private bool _visible;
    private bool _godMode;
    private List<string> _history = new List<string>();
    private int _historyIndex = -1;

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

        BuildConsoleUI();
        _consoleRoot.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            _visible = !_visible;
            _consoleRoot.SetActive(_visible);
            if (_visible)
            {
                _inputField.ActivateInputField();
                _inputField.Select();
            }
        }

        if (_visible && Keyboard.current != null)
        {
            // Enter to submit
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                string cmd = _inputField.text.Trim();
                if (!string.IsNullOrEmpty(cmd))
                {
                    ExecuteCommand(cmd);
                    _history.Add(cmd);
                    _historyIndex = _history.Count;
                    _inputField.text = "";
                }
                _inputField.ActivateInputField();
            }

            // Up/Down for history
            if (Keyboard.current.upArrowKey.wasPressedThisFrame && _history.Count > 0)
            {
                _historyIndex = Mathf.Max(0, _historyIndex - 1);
                _inputField.text = _history[_historyIndex];
                _inputField.caretPosition = _inputField.text.Length;
            }
            if (Keyboard.current.downArrowKey.wasPressedThisFrame && _history.Count > 0)
            {
                _historyIndex = Mathf.Min(_history.Count, _historyIndex + 1);
                _inputField.text = _historyIndex < _history.Count ? _history[_historyIndex] : "";
                _inputField.caretPosition = _inputField.text.Length;
            }
        }

        if (_godMode && _bowHealth != null && _bowHealth.CurrentHP < _bowHealth.MaxHP)
            _bowHealth.ResetHP();
    }

    void BuildConsoleUI()
    {
        // Canvas
        _consoleRoot = new GameObject("DebugConsole");
        _consoleRoot.transform.SetParent(transform);
        Canvas canvas = _consoleRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        _consoleRoot.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _consoleRoot.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
        _consoleRoot.AddComponent<GraphicRaycaster>();

        // Background panel — bottom half of screen
        GameObject panel = CreateUIObj("Panel", _consoleRoot.transform);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.85f);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 0);
        panelRT.anchorMax = new Vector2(1, 0.5f);
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Output scroll area
        GameObject scrollObj = CreateUIObj("Scroll", panel.transform);
        RectTransform scrollRT = scrollObj.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0, 0.08f);
        scrollRT.anchorMax = new Vector2(1, 1);
        scrollRT.offsetMin = new Vector2(16, 0);
        scrollRT.offsetMax = new Vector2(-16, -8);
        _scrollRect = scrollObj.AddComponent<ScrollRect>();
        _scrollRect.horizontal = false;
        scrollObj.AddComponent<Image>().color = new Color(0, 0, 0, 0); // transparent mask
        scrollObj.AddComponent<Mask>().showMaskGraphic = false;

        // Content for scroll
        GameObject content = CreateUIObj("Content", scrollObj.transform);
        RectTransform contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0, 1);
        contentRT.offsetMin = Vector2.zero;
        contentRT.offsetMax = Vector2.zero;
        ContentSizeFitter csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        _scrollRect.content = contentRT;

        // Output text
        GameObject textObj = CreateUIObj("OutputText", content.transform);
        _outputText = textObj.AddComponent<TextMeshProUGUI>();
        _outputText.fontSize = 28;
        _outputText.color = new Color(0.8f, 1f, 0.8f);
        _outputText.richText = true;
        _outputText.enableWordWrapping = true;
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;
        LayoutElement textLE = textObj.AddComponent<LayoutElement>();
        textLE.flexibleWidth = 1;

        // Input field — bottom bar
        GameObject inputObj = CreateUIObj("Input", panel.transform);
        RectTransform inputRT = inputObj.GetComponent<RectTransform>();
        inputRT.anchorMin = new Vector2(0, 0);
        inputRT.anchorMax = new Vector2(1, 0.08f);
        inputRT.offsetMin = new Vector2(8, 4);
        inputRT.offsetMax = new Vector2(-8, -4);
        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = new Color(0.15f, 0.15f, 0.15f, 1f);

        _inputField = inputObj.AddComponent<TMP_InputField>();

        // Text area for input
        GameObject inputTextArea = CreateUIObj("TextArea", inputObj.transform);
        RectTransform taRT = inputTextArea.GetComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(12, 0);
        taRT.offsetMax = new Vector2(-12, 0);
        inputTextArea.AddComponent<RectMask2D>();

        GameObject inputTextObj = CreateUIObj("Text", inputTextArea.transform);
        TextMeshProUGUI inputTMP = inputTextObj.AddComponent<TextMeshProUGUI>();
        inputTMP.fontSize = 30;
        inputTMP.color = Color.white;
        inputTMP.enableWordWrapping = false;
        RectTransform itRT = inputTextObj.GetComponent<RectTransform>();
        itRT.anchorMin = Vector2.zero;
        itRT.anchorMax = Vector2.one;
        itRT.offsetMin = Vector2.zero;
        itRT.offsetMax = Vector2.zero;

        // Placeholder
        GameObject placeholderObj = CreateUIObj("Placeholder", inputTextArea.transform);
        TextMeshProUGUI placeholder = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholder.text = "Type command... (help for list)";
        placeholder.fontSize = 30;
        placeholder.color = new Color(1, 1, 1, 0.3f);
        placeholder.fontStyle = FontStyles.Italic;
        placeholder.enableWordWrapping = false;
        RectTransform phRT = placeholderObj.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = Vector2.zero;
        phRT.offsetMax = Vector2.zero;

        _inputField.textViewport = taRT;
        _inputField.textComponent = inputTMP;
        _inputField.placeholder = placeholder;
        _inputField.fontAsset = inputTMP.font;

        // Welcome message
        Log("<color=yellow>== DEBUG CONSOLE (F1) ==</color>");
        Log("Type <color=white>help</color> for commands");
    }

    GameObject CreateUIObj(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    void Log(string msg)
    {
        _outputText.text += msg + "\n";
        // Auto scroll to bottom
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f;
    }

    void ExecuteCommand(string raw)
    {
        Log($"<color=#888>> {raw}</color>");

        string[] parts = raw.ToLower().Split(' ');
        string cmd = parts[0];
        int val = parts.Length > 1 && int.TryParse(parts[1], out int v) ? v : 0;

        switch (cmd)
        {
            case "help":
                Log("<color=yellow>Commands:</color>");
                Log("  <color=white>wave N</color>    — jump to wave N");
                Log("  <color=white>boss 1-5</color>  — boss (1=W10..5=W50)");
                Log("  <color=white>dmg N</color>     — add damage +N");
                Log("  <color=white>dur N</color>     — add durability +N");
                Log("  <color=white>arr N</color>     — add arrows +N");
                Log("  <color=white>coins N</color>   — add N coins");
                Log("  <color=white>god</color>       — toggle god mode");
                Log("  <color=white>kill</color>      — kill all enemies");
                Log("  <color=white>heal</color>      — heal bow");
                Log("  <color=white>arrows</color>    — refill arrows");
                Log("  <color=white>reset</color>     — reset upgrades");
                Log("  <color=white>clear</color>     — clear console");
                break;

            case "wave":
                if (val > 0) { JumpToWave(val); Log($"<color=green>Jumped to wave {val}</color>"); }
                else Log("<color=red>Usage: wave N</color>");
                break;

            case "boss":
                int bossWave = val switch { 1 => 10, 2 => 20, 3 => 30, 4 => 40, 5 => 50, _ => 0 };
                if (bossWave > 0) { JumpToWave(bossWave); Log($"<color=green>Jumped to boss wave {bossWave}</color>"); }
                else Log("<color=red>Usage: boss 1-5</color>");
                break;

            case "dmg":
                if (val > 0 && _arrowManager != null) { _arrowManager.AddBonusDamage(val); Log($"<color=green>Damage +{val} (total: +{_arrowManager.BonusDamage})</color>"); }
                else Log("<color=red>Usage: dmg N</color>");
                break;

            case "dur":
                if (val > 0 && _arrowManager != null) { _arrowManager.AddBonusDurability(val); Log($"<color=green>Durability +{val} (total: +{_arrowManager.BonusDurability})</color>"); }
                else Log("<color=red>Usage: dur N</color>");
                break;

            case "arr":
                if (val > 0 && _arrowManager != null) { _arrowManager.AddBonusArrowCount(val); _arrowManager.ResetArrows(); Log($"<color=green>Arrows +{val} (total: +{_arrowManager.BonusArrowCount})</color>"); }
                else Log("<color=red>Usage: arr N</color>");
                break;

            case "coins":
                if (val > 0 && _coinManager != null) { _coinManager.AddCoins(val); Log($"<color=green>+{val} coins</color>"); }
                else Log("<color=red>Usage: coins N</color>");
                break;

            case "god":
                _godMode = !_godMode;
                Log(_godMode ? "<color=green>GOD MODE ON</color>" : "<color=red>GOD MODE OFF</color>");
                break;

            case "kill":
                KillAllEnemies();
                Log("<color=green>All enemies killed</color>");
                break;

            case "heal":
                if (_bowHealth != null) { _bowHealth.ResetHP(); Log("<color=green>Bow healed</color>"); }
                break;

            case "arrows":
                if (_arrowManager != null) { _arrowManager.ResetArrows(); Log("<color=green>Arrows refilled</color>"); }
                break;

            case "reset":
                if (_arrowManager != null) { _arrowManager.ResetUpgrades(); _arrowManager.ResetArrows(); Log("<color=green>Upgrades reset</color>"); }
                break;

            case "clear":
                _outputText.text = "";
                break;

            case "status":
            case "info":
                int w = _waveManager != null ? _waveManager.CurrentWave : 0;
                string st = GameManager.Instance != null ? GameManager.Instance.CurrentState.ToString() : "?";
                Log($"Wave: {w} | State: {st}");
                if (_arrowManager != null)
                    Log($"Dmg+{_arrowManager.BonusDamage} Dur+{_arrowManager.BonusDurability} Arr+{_arrowManager.BonusArrowCount}");
                if (_bowHealth != null)
                    Log($"HP: {_bowHealth.CurrentHP}/{_bowHealth.MaxHP}");
                Log($"God: {(_godMode ? "ON" : "OFF")}");
                break;

            default:
                Log($"<color=red>Unknown command: {cmd}</color>");
                break;
        }
    }

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

    void KillAllEnemies()
    {
        if (_spawner == null) return;
        var list = new System.Collections.Generic.List<Enemy>(_spawner.ActiveEnemies);
        foreach (var e in list)
            if (e != null && e.Health != null && !e.Health.IsDead) e.Health.TakeDamage(9999);
    }
}
