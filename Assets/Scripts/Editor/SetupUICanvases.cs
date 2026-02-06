using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class SetupUICanvases
{
    [MenuItem("Tools/Setup UI Canvases")]
    public static void Execute()
    {
        string[] names = { "HUDCanvas", "UpgradeCanvas", "GameOverCanvas", "VictoryCanvas", "MainMenuCanvas", "PauseCanvas" };
        foreach (var n in names)
        {
            var existing = GameObject.Find(n);
            if (existing != null) Object.DestroyImmediate(existing);
        }

        CreateHUDCanvas();
        CreateUpgradeCanvas();
        CreateGameOverCanvas();
        CreateVictoryCanvas();
        CreateMainMenuCanvas();
        CreatePauseCanvas();

        Debug.Log("All UI Canvases created! Save the scene.");
    }

    // ===================== HUD =====================
    static void CreateHUDCanvas()
    {
        GameObject canvasGO = CreateCanvas("HUDCanvas", 0);

        // HUDContent wrapper (to hide all HUD elements at once)
        GameObject hudContent = CreateFullScreenPanel(canvasGO.transform, "HUDContent", new Color(0, 0, 0, 0));

        // HP Bar background (top-left)
        GameObject hpBarBg = CreatePanel(hudContent.transform, "HPBarBg",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(30, -30), new Vector2(300, 40),
            new Color(0.2f, 0.2f, 0.2f, 0.8f));

        GameObject hpFill = CreateStretchImage(hpBarBg.transform, "HPFill", new Color(0.2f, 0.8f, 0.2f, 1f));
        var fillImg = hpFill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;

        CreateStretchText(hpBarBg.transform, "HPText", "20/20", 24, Color.white, TextAlignmentOptions.Center);

        // Gem Row
        GameObject gemRow = CreatePanel(hudContent.transform, "GemRow",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(30, -80), new Vector2(180, 36),
            new Color(0.15f, 0.15f, 0.15f, 0.7f));
        CreateStretchText(gemRow.transform, "GemText", "0", 22, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        // Wave indicator (top-right)
        GameObject waveBg = CreatePanel(hudContent.transform, "WaveBg",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-30, -30), new Vector2(200, 40),
            new Color(0.15f, 0.15f, 0.15f, 0.7f));
        CreateStretchText(waveBg.transform, "WaveText", "VAGUE 1/4", 24, new Color(0.9f, 0.5f, 1f), TextAlignmentOptions.Center);

        // Arrow count (bottom-left)
        GameObject arrowBg = CreatePanel(hudContent.transform, "ArrowBg",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(30, 30), new Vector2(120, 40),
            new Color(0.15f, 0.15f, 0.15f, 0.7f));
        CreateStretchText(arrowBg.transform, "ArrowText", "x3", 26, new Color(0.4f, 0.9f, 1f), TextAlignmentOptions.Center);

        // Buff display (bottom center)
        GameObject buffRow = CreatePanel(hudContent.transform, "BuffRow",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(0, 30), new Vector2(360, 36),
            new Color(0f, 0f, 0f, 0f));
        CreateAnchoredText(buffRow.transform, "BuffDurability", new Vector2(0f, 0f), new Vector2(0.33f, 1f), "", 18, new Color(0.4f, 0.9f, 0.4f), TextAlignmentOptions.Center);
        CreateAnchoredText(buffRow.transform, "BuffArrows", new Vector2(0.33f, 0f), new Vector2(0.66f, 1f), "", 18, new Color(0.4f, 0.8f, 1f), TextAlignmentOptions.Center);
        CreateAnchoredText(buffRow.transform, "BuffDamage", new Vector2(0.66f, 0f), new Vector2(1f, 1f), "", 18, new Color(1f, 0.5f, 0.3f), TextAlignmentOptions.Center);

        // Pause button (top-right, below wave)
        GameObject pauseBtn = CreateButtonGO(canvasGO.transform, "PauseButton",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-30, -80), new Vector2(80, 36),
            new Color(0.3f, 0.3f, 0.3f, 0.7f));
        CreateStretchText(pauseBtn.transform, "PauseText", "II", 22, Color.white, TextAlignmentOptions.Center);

        // Wave Complete Banner
        GameObject banner = CreateFullScreenPanel(canvasGO.transform, "WaveCompleteBanner", new Color(0f, 0f, 0f, 0.7f));
        CreateAnchoredText(banner.transform, "BannerText",
            new Vector2(0.05f, 0.35f), new Vector2(0.95f, 0.65f),
            "VAGUE 1 TERMINEE !", 52, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);
        banner.SetActive(false);

        canvasGO.AddComponent<HUDManager>();
    }

    // ===================== UPGRADE =====================
    static void CreateUpgradeCanvas()
    {
        GameObject canvasGO = CreateCanvas("UpgradeCanvas", 10);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "UpgradePanel", new Color(0f, 0f, 0f, 0.85f));

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.85f),
            "VAGUE 1 TERMINEE !", 42, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        CreateAnchoredText(panel.transform, "GemsInfo",
            new Vector2(0.1f, 0.68f), new Vector2(0.9f, 0.75f),
            "Gemmes: +0  (Total: 0)", 26, Color.white, TextAlignmentOptions.Center);

        float btnTop = 0.62f, btnHeight = 0.1f, btnGap = 0.02f;
        Color btnColor = new Color(0.15f, 0.2f, 0.35f, 0.95f);

        for (int i = 0; i < 3; i++)
        {
            float top = btnTop - i * (btnHeight + btnGap);
            float bot = top - btnHeight;

            GameObject btnGO = CreateButtonGO(panel.transform, $"UpgradeBtn{i}",
                new Vector2(0.08f, bot), new Vector2(0.92f, top), btnColor);

            var btn = btnGO.GetComponent<Button>();
            var colors = btn.colors;
            colors.normalColor = btnColor;
            colors.highlightedColor = new Color(0.25f, 0.35f, 0.55f, 1f);
            colors.pressedColor = new Color(0.1f, 0.15f, 0.25f, 1f);
            colors.disabledColor = new Color(0.15f, 0.15f, 0.15f, 0.6f);
            btn.colors = colors;

            CreateAnchoredText(btnGO.transform, "Title",
                new Vector2(0.05f, 0f), new Vector2(0.75f, 1f),
                "+1 Upgrade\n<size=18>Description</size>", 26, Color.white, TextAlignmentOptions.MidlineLeft);
            CreateAnchoredText(btnGO.transform, "Cost",
                new Vector2(0.75f, 0f), new Vector2(0.95f, 1f),
                "10", 28, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);
        }

        float skipTop = btnTop - 3 * (btnHeight + btnGap);
        GameObject skipGO = CreateButtonGO(panel.transform, "SkipButton",
            new Vector2(0.2f, skipTop - 0.06f), new Vector2(0.8f, skipTop),
            new Color(0.3f, 0.3f, 0.3f, 0.9f));
        CreateAnchoredText(skipGO.transform, "SkipText", Vector2.zero, Vector2.one,
            "PASSER", 24, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<UpgradeScreenUI>();
    }

    // ===================== GAME OVER =====================
    static void CreateGameOverCanvas()
    {
        GameObject canvasGO = CreateCanvas("GameOverCanvas", 20);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "GameOverPanel", new Color(0.1f, 0f, 0f, 0.9f));

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.7f), new Vector2(0.9f, 0.85f),
            "GAME OVER", 56, new Color(0.9f, 0.2f, 0.2f), TextAlignmentOptions.Center);

        CreateAnchoredText(panel.transform, "WaveStat",
            new Vector2(0.1f, 0.58f), new Vector2(0.9f, 0.65f),
            "Vague atteinte : 1", 26, Color.white, TextAlignmentOptions.Center);
        CreateAnchoredText(panel.transform, "GemsStat",
            new Vector2(0.1f, 0.51f), new Vector2(0.9f, 0.58f),
            "Gemmes : 0", 26, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);
        CreateAnchoredText(panel.transform, "KillsStat",
            new Vector2(0.1f, 0.44f), new Vector2(0.9f, 0.51f),
            "Ennemis tues : 0", 26, Color.white, TextAlignmentOptions.Center);
        CreateAnchoredText(panel.transform, "ArrowsStat",
            new Vector2(0.1f, 0.37f), new Vector2(0.9f, 0.44f),
            "Fleches tirees : 0", 26, Color.white, TextAlignmentOptions.Center);

        CreateButtonGO(panel.transform, "ReplayButton",
            new Vector2(0.15f, 0.2f), new Vector2(0.48f, 0.3f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        CreateAnchoredText(panel.transform.Find("ReplayButton"), "ReplayText",
            Vector2.zero, Vector2.one, "REJOUER", 26, Color.white, TextAlignmentOptions.Center);

        CreateButtonGO(panel.transform, "MenuButton",
            new Vector2(0.52f, 0.2f), new Vector2(0.85f, 0.3f),
            new Color(0.3f, 0.3f, 0.3f, 0.95f));
        CreateAnchoredText(panel.transform.Find("MenuButton"), "MenuText",
            Vector2.zero, Vector2.one, "MENU", 26, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<GameOverScreenUI>();
    }

    // ===================== VICTORY =====================
    static void CreateVictoryCanvas()
    {
        GameObject canvasGO = CreateCanvas("VictoryCanvas", 20);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "VictoryPanel", new Color(0f, 0.05f, 0.1f, 0.9f));

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.7f), new Vector2(0.9f, 0.85f),
            "VICTOIRE !", 56, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        CreateAnchoredText(panel.transform, "GemsStat",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.62f),
            "Gemmes : 0", 26, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);
        CreateAnchoredText(panel.transform, "KillsStat",
            new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.55f),
            "Ennemis tues : 0", 26, Color.white, TextAlignmentOptions.Center);
        CreateAnchoredText(panel.transform, "ArrowsStat",
            new Vector2(0.1f, 0.41f), new Vector2(0.9f, 0.48f),
            "Fleches tirees : 0", 26, Color.white, TextAlignmentOptions.Center);

        CreateButtonGO(panel.transform, "ReplayButton",
            new Vector2(0.15f, 0.22f), new Vector2(0.48f, 0.32f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        CreateAnchoredText(panel.transform.Find("ReplayButton"), "ReplayText",
            Vector2.zero, Vector2.one, "REJOUER", 26, Color.white, TextAlignmentOptions.Center);

        CreateButtonGO(panel.transform, "MenuButton",
            new Vector2(0.52f, 0.22f), new Vector2(0.85f, 0.32f),
            new Color(0.3f, 0.3f, 0.3f, 0.95f));
        CreateAnchoredText(panel.transform.Find("MenuButton"), "MenuText",
            Vector2.zero, Vector2.one, "MENU", 26, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<VictoryScreenUI>();
    }

    // ===================== MAIN MENU =====================
    static void CreateMainMenuCanvas()
    {
        GameObject canvasGO = CreateCanvas("MainMenuCanvas", 30);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "MainMenuPanel", new Color(0.05f, 0.05f, 0.15f, 0.95f));

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.8f),
            "ARROW\nSTRIKE", 72, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        GameObject playBtn = CreateButtonGO(panel.transform, "PlayButton",
            new Vector2(0.2f, 0.3f), new Vector2(0.8f, 0.42f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        var btnComp = playBtn.GetComponent<Button>();
        var c = btnComp.colors;
        c.highlightedColor = new Color(0.3f, 0.8f, 0.4f);
        c.pressedColor = new Color(0.15f, 0.4f, 0.2f);
        btnComp.colors = c;
        CreateStretchText(playBtn.transform, "PlayText", "JOUER", 36, Color.white, TextAlignmentOptions.Center);

        canvasGO.AddComponent<MainMenuUI>();
    }

    // ===================== PAUSE =====================
    static void CreatePauseCanvas()
    {
        GameObject canvasGO = CreateCanvas("PauseCanvas", 25);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "PausePanel", new Color(0f, 0f, 0f, 0.75f));

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.7f),
            "PAUSE", 56, Color.white, TextAlignmentOptions.Center);

        CreateButtonGO(panel.transform, "ResumeButton",
            new Vector2(0.2f, 0.38f), new Vector2(0.8f, 0.48f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        CreateAnchoredText(panel.transform.Find("ResumeButton"), "ResumeText",
            Vector2.zero, Vector2.one, "REPRENDRE", 28, Color.white, TextAlignmentOptions.Center);

        CreateButtonGO(panel.transform, "QuitButton",
            new Vector2(0.2f, 0.25f), new Vector2(0.8f, 0.35f),
            new Color(0.5f, 0.2f, 0.2f, 0.95f));
        CreateAnchoredText(panel.transform.Find("QuitButton"), "QuitText",
            Vector2.zero, Vector2.one, "QUITTER", 28, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<PauseScreenUI>();
    }

    // ===================== HELPERS =====================

    static GameObject CreateCanvas(string name, int sortOrder)
    {
        GameObject go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static GameObject CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        go.GetComponent<Image>().color = color;
        return go;
    }

    static GameObject CreateFullScreenPanel(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = color;
        return go;
    }

    static GameObject CreateStretchImage(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = color;
        return go;
    }

    static TextMeshProUGUI CreateStretchText(Transform parent, string name,
        string text, int fontSize, Color color, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.fontStyle = FontStyles.Bold;
        return tmp;
    }

    static TextMeshProUGUI CreateAnchoredText(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        string text, int fontSize, Color color, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = align;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = true;
        tmp.richText = true;
        return tmp;
    }

    static GameObject CreateButtonGO(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color bgColor)
    {
        return CreateButtonGO(parent, name, anchorMin, anchorMax,
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, bgColor);
    }

    static GameObject CreateButtonGO(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 size, Color bgColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        rt.offsetMin = rt.offsetMin; // keep offset for anchor-based ones
        if (size == Vector2.zero)
        {
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        go.GetComponent<Image>().color = bgColor;
        return go;
    }
}
