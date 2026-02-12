using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;

public static class SetupEnemyVariants
{
    [MenuItem("Monster Cannon/Setup Enemy Data + Variants")]
    public static void Execute()
    {
        // 1. Ensure Resources/EnemyData folder
        EnsureFolder("Assets/Resources");
        EnsureFolder("Assets/Resources/EnemyData");

        // 2. Move existing EnemyData to Resources (if not already there)
        MoveToResources("Assets/ScriptableObjects/EnemyData/EnemyData_Weak.asset");
        MoveToResources("Assets/ScriptableObjects/EnemyData/EnemyData_Medium.asset");
        MoveToResources("Assets/ScriptableObjects/EnemyData/EnemyData_Strong.asset");

        // 3. Create EnemyData_Elite if it doesn't exist
        CreateEliteData();

        // 4. Update EnemyData_Strong values
        UpdateStrongData();

        // 5. Assign visual variants to all EnemyData
        AssignVariants();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SetupEnemyVariants COMPLETE");
    }

    static void MoveToResources(string sourcePath)
    {
        string fileName = Path.GetFileName(sourcePath);
        string destPath = $"Assets/Resources/EnemyData/{fileName}";

        if (AssetDatabase.LoadAssetAtPath<EnemyData>(destPath) != null)
        {
            Debug.Log($"{fileName}: already in Resources, skipping move.");
            return;
        }

        if (AssetDatabase.LoadAssetAtPath<EnemyData>(sourcePath) == null)
        {
            Debug.LogWarning($"{fileName}: not found at {sourcePath}");
            return;
        }

        string result = AssetDatabase.MoveAsset(sourcePath, destPath);
        if (string.IsNullOrEmpty(result))
            Debug.Log($"Moved {fileName} to Resources/EnemyData/");
        else
            Debug.LogError($"Failed to move {fileName}: {result}");
    }

    static void CreateEliteData()
    {
        string path = "Assets/Resources/EnemyData/EnemyData_Elite.asset";
        if (AssetDatabase.LoadAssetAtPath<EnemyData>(path) != null)
        {
            Debug.Log("EnemyData_Elite: already exists, skipping creation.");
            return;
        }

        EnemyData elite = ScriptableObject.CreateInstance<EnemyData>();
        elite.enemyName = "Elite";
        elite.maxHP = 5;
        elite.damage = 4;
        elite.color = new Color(0.6f, 0.2f, 0.8f, 1f); // violet
        elite.coinDropMin = 8;
        elite.coinDropMax = 12;

        // Default sprite: Monster02 first frame
        elite.sprite = LoadFirstFrame("Monster02");

        // Default anim controller: Monster02
        elite.animController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
            "Assets/Animations/Monsters/Monster02.controller");

        AssetDatabase.CreateAsset(elite, path);
        Debug.Log("Created EnemyData_Elite");
    }

    static void UpdateStrongData()
    {
        EnemyData strong = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Resources/EnemyData/EnemyData_Strong.asset");
        if (strong == null)
        {
            Debug.LogWarning("EnemyData_Strong not found for update");
            return;
        }

        strong.damage = 3;
        strong.coinDropMin = 4;
        strong.coinDropMax = 6;
        EditorUtility.SetDirty(strong);
        Debug.Log("Updated EnemyData_Strong: damage=3, coins=4-6");
    }

    static void AssignVariants()
    {
        // Weak: Monster01 (green) + Monster06 (pink)
        AssignVariantsTo("EnemyData_Weak", new[] { "Monster01", "Monster06" });

        // Medium: Monster03 (cyan) + Monster07 (blue 3 eyes) + Monster09 (orange)
        AssignVariantsTo("EnemyData_Medium", new[] { "Monster03", "Monster07", "Monster09" });

        // Strong: Monster05 (violet spikes) + Monster04 (violet striped)
        AssignVariantsTo("EnemyData_Strong", new[] { "Monster05", "Monster04" });

        // Elite: Monster02 (blue spiky) + Monster08 (pink striped) + Monster10 (green dark)
        AssignVariantsTo("EnemyData_Elite", new[] { "Monster02", "Monster08", "Monster10" });
    }

    static void AssignVariantsTo(string dataName, string[] monsterNames)
    {
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>($"Assets/Resources/EnemyData/{dataName}.asset");
        if (data == null)
        {
            Debug.LogWarning($"{dataName}: not found for variant assignment");
            return;
        }

        var variants = new EnemyData.VisualVariant[monsterNames.Length];
        for (int i = 0; i < monsterNames.Length; i++)
        {
            string monsterName = monsterNames[i];
            variants[i] = new EnemyData.VisualVariant
            {
                sprite = LoadFirstFrame(monsterName),
                animController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                    $"Assets/Animations/Monsters/{monsterName}.controller")
            };

            if (variants[i].animController == null)
                Debug.LogWarning($"{monsterName}: AnimatorController not found! Run 'Setup All Monster Animations' first.");
        }

        data.variants = variants;
        EditorUtility.SetDirty(data);
        Debug.Log($"{dataName}: assigned {monsterNames.Length} visual variants ({string.Join(", ", monsterNames)})");
    }

    static Sprite LoadFirstFrame(string monsterName)
    {
        string folder = $"Assets/Art/Sprites/Png/Monster/{monsterName}";
        string prefix = $"{monsterName}-animation_";

        if (!AssetDatabase.IsValidFolder(folder)) return null;

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folder });
        string firstPath = guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Where(p => Path.GetFileName(p).StartsWith(prefix))
            .OrderBy(p => p)
            .FirstOrDefault();

        if (firstPath == null) return null;
        return AssetDatabase.LoadAssetAtPath<Sprite>(firstPath);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string name = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, name);
    }
}
