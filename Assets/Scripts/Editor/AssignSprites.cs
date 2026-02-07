using UnityEditor;
using UnityEngine;

public static class AssignSprites
{
    [MenuItem("Tools/Assign Sprites")]
    public static string Execute()
    {
        ConfigureSpriteImports();
        AssetDatabase.Refresh();

        AssignBowSprite();
        AssignArrowSprite();
        AssignEnemySprites();
        CreateBackground();

        AssetDatabase.SaveAssets();
        return "All sprites assigned successfully!";
    }

    static void ConfigureSpriteImports()
    {
        string[] folders = {
            "Assets/Art/Sprites/Png/Guns",
            "Assets/Art/Sprites/Png/Bullets",
            "Assets/Art/Sprites/Png/Monster",
            "Assets/Art/Sprites/Png/Gameplay Area",
            "Assets/Art/Sprites/Png/Shoot fx",
            "Assets/Art/Sprites/Png/Dead fx"
        };

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", folders);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
        }
        Debug.Log($"Configured {guids.Length} sprite imports.");
    }

    static void AssignBowSprite()
    {
        GameObject bow = GameObject.Find("Bow");
        if (bow == null) { Debug.LogError("Bow not found in scene!"); return; }

        Sprite gunSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Art/Sprites/Png/Guns/Gun01/Idle/Gun01-Idle_0.png");
        if (gunSprite == null) { Debug.LogError("Gun01-Idle_0.png not found!"); return; }

        SpriteRenderer sr = bow.GetComponent<SpriteRenderer>();
        if (sr == null) sr = bow.AddComponent<SpriteRenderer>();

        sr.sprite = gunSprite;
        sr.color = Color.white;

        // Scale to ~1.5 units wide (gun sprite is roughly 200px wide at 100ppu = 2 units)
        bow.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

        EditorUtility.SetDirty(bow);
        Debug.Log("Bow sprite assigned: Gun01-Idle_0");
    }

    static void AssignArrowSprite()
    {
        string prefabPath = "Assets/Prefabs/Arrow.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) { Debug.LogError("Arrow.prefab not found!"); return; }

        Sprite bulletSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Art/Sprites/Png/Bullets/1.png");
        if (bulletSprite == null) { Debug.LogError("Bullet 1.png not found!"); return; }

        // Modify prefab
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        GameObject instance = PrefabUtility.LoadPrefabContents(assetPath);

        SpriteRenderer sr = instance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = bulletSprite;
            sr.color = Color.white;
        }

        // Adjust scale for bullet (was 0.1x0.5 for square, bullet sprite is ~100px at 100ppu)
        instance.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

        // Adjust collider to match new sprite
        CircleCollider2D col = instance.GetComponent<CircleCollider2D>();
        if (col != null) col.radius = 0.3f;

        PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
        PrefabUtility.UnloadPrefabContents(instance);

        Debug.Log("Arrow prefab sprite assigned: Bullet 1");
    }

    static void AssignEnemySprites()
    {
        // Map: EnemyData asset → monster sprite
        var mapping = new (string dataPath, string spritePath)[] {
            ("Assets/ScriptableObjects/EnemyData/EnemyData_Weak.asset",
             "Assets/Art/Sprites/Png/Monster/Monster01/Monster01-animation_00.png"),
            ("Assets/ScriptableObjects/EnemyData/EnemyData_Medium.asset",
             "Assets/Art/Sprites/Png/Monster/Monster03/Monster03-animation_00.png"),
            ("Assets/ScriptableObjects/EnemyData/EnemyData_Strong.asset",
             "Assets/Art/Sprites/Png/Monster/Monster05/Monster05-animation_00.png"),
        };

        foreach (var (dataPath, spritePath) in mapping)
        {
            EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(dataPath);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            if (data == null) { Debug.LogWarning($"EnemyData not found: {dataPath}"); continue; }
            if (sprite == null) { Debug.LogWarning($"Monster sprite not found: {spritePath}"); continue; }

            data.sprite = sprite;
            data.color = Color.white; // Use sprite's own colors instead of tint
            EditorUtility.SetDirty(data);
            Debug.Log($"Enemy sprite assigned: {data.enemyName} → {sprite.name}");
        }

        // Also adjust enemy prefab scale
        string enemyPrefabPath = "Assets/Prefabs/Enemies/Enemy.prefab";
        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(enemyPrefabPath);
        if (enemyPrefab != null)
        {
            GameObject instance = PrefabUtility.LoadPrefabContents(enemyPrefabPath);
            instance.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            // Adjust collider
            var col = instance.GetComponent<CircleCollider2D>();
            if (col != null) col.radius = 0.7f;
            var boxCol = instance.GetComponent<BoxCollider2D>();
            if (boxCol != null) boxCol.size = new Vector2(1.4f, 1.4f);

            PrefabUtility.SaveAsPrefabAsset(instance, enemyPrefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            Debug.Log("Enemy prefab scale adjusted.");
        }
    }

    static void CreateBackground()
    {
        // Remove existing background if any
        GameObject existingBg = GameObject.Find("GameBackground");
        if (existingBg != null) Object.DestroyImmediate(existingBg);

        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Art/Sprites/Png/Gameplay Area/game area.png");
        if (bgSprite == null) { Debug.LogError("game area.png not found!"); return; }

        GameObject bg = new GameObject("GameBackground");
        SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = bgSprite;
        sr.sortingOrder = -100;

        // Scale to cover the camera view (~10 units wide, ~18 units tall for 9:16)
        float spriteWidth = bgSprite.bounds.size.x;
        float spriteHeight = bgSprite.bounds.size.y;
        float targetWidth = 12f;
        float scale = targetWidth / spriteWidth;
        bg.transform.localScale = new Vector3(scale, scale, 1f);

        // Center vertically (camera is at 0,0 but game area is slightly offset)
        bg.transform.position = new Vector3(0f, 0f, 0f);

        EditorUtility.SetDirty(bg);
        Debug.Log($"Background created: scale={scale:F2}, size={spriteWidth*scale:F1}x{spriteHeight*scale:F1}");
    }
}
