using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class SetupUICanvases
{
    const string UI = "Assets/Art/Sprites/Png/User interfaces/";


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

        GameObject hudContent = CreateFullScreenPanel(canvasGO.transform, "HUDContent", new Color(0, 0, 0, 0));

        // HP Bar background (top-left) with sprite
        GameObject hpBarBg = CreatePanel(hudContent.transform, "HPBarBg",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(30, -30), new Vector2(300, 40),
            new Color(0.2f, 0.2f, 0.2f, 0.8f));
        ApplySprite(hpBarBg, UI + "enemy hp bar/enemy hp bar bg.png");

        // HP Fill with sprite (padded to fit inside the bg frame)
        GameObject hpFill = new GameObject("HPFill", typeof(RectTransform), typeof(Image));
        hpFill.transform.SetParent(hpBarBg.transform, false);
        var hpFillRt = hpFill.GetComponent<RectTransform>();
        hpFillRt.anchorMin = Vector2.zero;
        hpFillRt.anchorMax = Vector2.one;
        hpFillRt.offsetMin = new Vector2(6, 4);
        hpFillRt.offsetMax = new Vector2(-6, -4);
        ApplySprite(hpFill, UI + "enemy hp bar/enemy hp bar fg.png");
        var fillImg = hpFill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 1f;

        CreateStretchText(hpBarBg.transform, "HPText", "20/20", 24, Color.white, TextAlignmentOptions.Center);

        // Gem Row with sprite
        GameObject gemRow = CreatePanel(hudContent.transform, "GemRow",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(30, -80), new Vector2(180, 36),
            new Color(0.15f, 0.15f, 0.15f, 0.7f));
        ApplySprite(gemRow, UI + "game play area Ui/money bar.png");
        CreateStretchText(gemRow.transform, "GemText", "0", 22, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        // Wave indicator (top-right) with sprite
        GameObject waveBg = CreatePanel(hudContent.transform, "WaveBg",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-30, -30), new Vector2(200, 40),
            new Color(0.15f, 0.15f, 0.15f, 0.7f));
        ApplySprite(waveBg, UI + "game play area Ui/money bar.png");
        CreateStretchText(waveBg.transform, "WaveText", "VAGUE 1/4", 24, new Color(0.9f, 0.5f, 1f), TextAlignmentOptions.Center);

        // Arrow count (bottom-left) with sprite
        GameObject arrowBg = CreatePanel(hudContent.transform, "ArrowBg",
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(30, 30), new Vector2(120, 40),
            new Color(0.15f, 0.15f, 0.15f, 0.7f));
        ApplySprite(arrowBg, UI + "game play area Ui/money bar.png");
        CreateStretchText(arrowBg.transform, "ArrowText", "x3", 26, new Color(0.4f, 0.9f, 1f), TextAlignmentOptions.Center);

        // Buff display (bottom center)
        GameObject buffRow = CreatePanel(hudContent.transform, "BuffRow",
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0),
            new Vector2(0, 30), new Vector2(360, 36),
            new Color(0f, 0f, 0f, 0f));
        CreateAnchoredText(buffRow.transform, "BuffDurability", new Vector2(0f, 0f), new Vector2(0.33f, 1f), "", 18, new Color(0.4f, 0.9f, 0.4f), TextAlignmentOptions.Center);
        CreateAnchoredText(buffRow.transform, "BuffArrows", new Vector2(0.33f, 0f), new Vector2(0.66f, 1f), "", 18, new Color(0.4f, 0.8f, 1f), TextAlignmentOptions.Center);
        CreateAnchoredText(buffRow.transform, "BuffDamage", new Vector2(0.66f, 0f), new Vector2(1f, 1f), "", 18, new Color(1f, 0.5f, 0.3f), TextAlignmentOptions.Center);

        // Pause button with sprite
        GameObject pauseBtn = CreateButtonGO(canvasGO.transform, "PauseButton",
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-30, -80), new Vector2(80, 36),
            new Color(0.3f, 0.3f, 0.3f, 0.7f));
        ApplyButtonSprites(pauseBtn, UI + "game play area Ui/btn01.png", UI + "game play area Ui/btn01 pressed.png");
        CreateStretchText(pauseBtn.transform, "PauseText", "II", 22, Color.white, TextAlignmentOptions.Center);

        // Wave Complete Banner
        GameObject banner = CreateFullScreenPanel(canvasGO.transform, "WaveCompleteBanner", new Color(0f, 0f, 0f, 0.7f));
        ApplySprite(banner, UI + "wave cleared popup/dark background.png");

        // Wave cleared box in center
        GameObject waveBox = new GameObject("WaveBox", typeof(RectTransform), typeof(Image));
        waveBox.transform.SetParent(banner.transform, false);
        var waveBoxRt = waveBox.GetComponent<RectTransform>();
        waveBoxRt.anchorMin = new Vector2(0.05f, 0.3f);
        waveBoxRt.anchorMax = new Vector2(0.95f, 0.7f);
        waveBoxRt.offsetMin = Vector2.zero;
        waveBoxRt.offsetMax = Vector2.zero;
        ApplySprite(waveBox, UI + "wave cleared popup/wave cleared box.png");
        waveBox.GetComponent<Image>().preserveAspect = true;

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
        ApplySprite(panel, UI + "free upgrade popup/dark background.png");

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.85f),
            "VAGUE 1 TERMINEE !", 42, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        CreateAnchoredText(panel.transform, "GemsInfo",
            new Vector2(0.1f, 0.68f), new Vector2(0.9f, 0.75f),
            "Gemmes: +0  (Total: 0)", 26, Color.white, TextAlignmentOptions.Center);

        float btnTop = 0.62f, btnHeight = 0.1f, btnGap = 0.02f;

        for (int i = 0; i < 3; i++)
        {
            float top = btnTop - i * (btnHeight + btnGap);
            float bot = top - btnHeight;

            GameObject btnGO = CreateButtonGO(panel.transform, $"UpgradeBtn{i}",
                new Vector2(0.08f, bot), new Vector2(0.92f, top),
                new Color(0.15f, 0.2f, 0.35f, 0.95f));
            ApplyButtonSprites(btnGO, UI + "game play area Ui/btn05.png", UI + "game play area Ui/btn05 pressed.png");

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
        ApplyButtonSprites(skipGO, UI + "game play area Ui/btn03.png", UI + "game play area Ui/btn03 pressed.png");
        CreateAnchoredText(skipGO.transform, "SkipText", Vector2.zero, Vector2.one,
            "PASSER", 24, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<UpgradeScreenUI>();
    }

    // ===================== GAME OVER =====================
    static void CreateGameOverCanvas()
    {
        GameObject canvasGO = CreateCanvas("GameOverCanvas", 20);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "GameOverPanel", new Color(0.1f, 0f, 0f, 0.9f));
        ApplySprite(panel, UI + "wave cleared popup/dark background.png");

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
            "Tirs effectues : 0", 26, Color.white, TextAlignmentOptions.Center);

        GameObject replayBtn = CreateButtonGO(panel.transform, "ReplayButton",
            new Vector2(0.15f, 0.2f), new Vector2(0.48f, 0.3f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        ApplyButtonSprites(replayBtn, UI + "game play area Ui/btn05.png", UI + "game play area Ui/btn05 pressed.png");
        CreateAnchoredText(replayBtn.transform, "ReplayText",
            Vector2.zero, Vector2.one, "REJOUER", 26, Color.white, TextAlignmentOptions.Center);

        GameObject menuBtn = CreateButtonGO(panel.transform, "MenuButton",
            new Vector2(0.52f, 0.2f), new Vector2(0.85f, 0.3f),
            new Color(0.3f, 0.3f, 0.3f, 0.95f));
        ApplyButtonSprites(menuBtn, UI + "game play area Ui/btn03.png", UI + "game play area Ui/btn03 pressed.png");
        CreateAnchoredText(menuBtn.transform, "MenuText",
            Vector2.zero, Vector2.one, "MENU", 26, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<GameOverScreenUI>();
    }

    // ===================== VICTORY =====================
    static void CreateVictoryCanvas()
    {
        GameObject canvasGO = CreateCanvas("VictoryCanvas", 20);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "VictoryPanel", new Color(0f, 0.05f, 0.1f, 0.9f));
        ApplySprite(panel, UI + "wave cleared popup/dark background.png");

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
            "Tirs effectues : 0", 26, Color.white, TextAlignmentOptions.Center);

        GameObject replayBtn = CreateButtonGO(panel.transform, "ReplayButton",
            new Vector2(0.15f, 0.22f), new Vector2(0.48f, 0.32f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        ApplyButtonSprites(replayBtn, UI + "game play area Ui/btn05.png", UI + "game play area Ui/btn05 pressed.png");
        CreateAnchoredText(replayBtn.transform, "ReplayText",
            Vector2.zero, Vector2.one, "REJOUER", 26, Color.white, TextAlignmentOptions.Center);

        GameObject menuBtn = CreateButtonGO(panel.transform, "MenuButton",
            new Vector2(0.52f, 0.22f), new Vector2(0.85f, 0.32f),
            new Color(0.3f, 0.3f, 0.3f, 0.95f));
        ApplyButtonSprites(menuBtn, UI + "game play area Ui/btn03.png", UI + "game play area Ui/btn03 pressed.png");
        CreateAnchoredText(menuBtn.transform, "MenuText",
            Vector2.zero, Vector2.one, "MENU", 26, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<VictoryScreenUI>();
    }

    // ===================== MAIN MENU =====================
    static void CreateMainMenuCanvas()
    {
        GameObject canvasGO = CreateCanvas("MainMenuCanvas", 30);

        // Background with cover sprite
        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "MainMenuPanel", new Color(0.05f, 0.05f, 0.15f, 0.95f));
        ApplySprite(panel, UI + "landing Screen/cover background.png");

        // Title image
        GameObject titleImg = new GameObject("TitleImage", typeof(RectTransform), typeof(Image));
        titleImg.transform.SetParent(panel.transform, false);
        var titleRt = titleImg.GetComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.1f, 0.55f);
        titleRt.anchorMax = new Vector2(0.9f, 0.8f);
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;
        var titleImgComp = titleImg.GetComponent<Image>();
        Sprite titleSprite = LoadSprite(UI + "landing Screen/game title bg.png");
        if (titleSprite != null)
        {
            titleImgComp.sprite = titleSprite;
            titleImgComp.color = Color.white;
            titleImgComp.preserveAspect = true;
        }
        else
        {
            titleImgComp.color = new Color(0, 0, 0, 0);
        }

        // Keep text title on top of image for our custom game name
        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.8f),
            "MONSTER\nCANNON", 72, new Color(1f, 0.85f, 0.2f), TextAlignmentOptions.Center);

        // Play button with sprite
        GameObject playBtn = CreateButtonGO(panel.transform, "PlayButton",
            new Vector2(0.2f, 0.3f), new Vector2(0.8f, 0.42f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        ApplyButtonSprites(playBtn, UI + "landing Screen/start game btn.png", UI + "landing Screen/start game btn pressed.png");
        CreateStretchText(playBtn.transform, "PlayText", "JOUER", 36, Color.white, TextAlignmentOptions.Center);

        canvasGO.AddComponent<MainMenuUI>();
    }

    // ===================== PAUSE =====================
    static void CreatePauseCanvas()
    {
        GameObject canvasGO = CreateCanvas("PauseCanvas", 25);

        GameObject panel = CreateFullScreenPanel(canvasGO.transform, "PausePanel", new Color(0f, 0f, 0f, 0.75f));
        ApplySprite(panel, UI + "wave cleared popup/dark background.png");

        CreateAnchoredText(panel.transform, "Title",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.7f),
            "PAUSE", 56, Color.white, TextAlignmentOptions.Center);

        GameObject resumeBtn = CreateButtonGO(panel.transform, "ResumeButton",
            new Vector2(0.2f, 0.38f), new Vector2(0.8f, 0.48f),
            new Color(0.2f, 0.6f, 0.3f, 0.95f));
        ApplyButtonSprites(resumeBtn, UI + "game play area Ui/btn05.png", UI + "game play area Ui/btn05 pressed.png");
        CreateAnchoredText(resumeBtn.transform, "ResumeText",
            Vector2.zero, Vector2.one, "REPRENDRE", 28, Color.white, TextAlignmentOptions.Center);

        GameObject quitBtn = CreateButtonGO(panel.transform, "QuitButton",
            new Vector2(0.2f, 0.25f), new Vector2(0.8f, 0.35f),
            new Color(0.5f, 0.2f, 0.2f, 0.95f));
        ApplyButtonSprites(quitBtn, UI + "game play area Ui/btn06.png", UI + "game play area Ui/btn06 pressed.png");
        CreateAnchoredText(quitBtn.transform, "QuitText",
            Vector2.zero, Vector2.one, "QUITTER", 28, Color.white, TextAlignmentOptions.Center);

        panel.SetActive(false);
        canvasGO.AddComponent<PauseScreenUI>();
    }

    // ===================== SPRITE HELPERS =====================

    static Sprite LoadSprite(string path)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static void ApplySprite(GameObject go, string path)
    {
        Sprite sprite = LoadSprite(path);
        if (sprite == null) return;
        var img = go.GetComponent<Image>();
        if (img == null) return;
        img.sprite = sprite;
        img.color = Color.white;
    }

    static void ApplyButtonSprites(GameObject btnGO, string normalPath, string pressedPath)
    {
        Sprite normal = LoadSprite(normalPath);
        if (normal == null) return;

        var img = btnGO.GetComponent<Image>();
        img.sprite = normal;
        img.color = Color.white;

        var btn = btnGO.GetComponent<Button>();
        btn.transition = Selectable.Transition.SpriteSwap;
        var spriteState = new SpriteState();
        spriteState.pressedSprite = LoadSprite(pressedPath);
        btn.spriteState = spriteState;
    }

    // ===================== BASE HELPERS =====================

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
        tmp.textWrappingMode = TextWrappingModes.Normal;
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
        rt.offsetMin = rt.offsetMin;
        if (size == Vector2.zero)
        {
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        go.GetComponent<Image>().color = bgColor;
        return go;
    }
}
