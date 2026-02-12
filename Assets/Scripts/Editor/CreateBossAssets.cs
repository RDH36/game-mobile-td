using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class CreateBossAssets
{
    [MenuItem("Monster Cannon/Create Boss Assets")]
    public static void Execute()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/BossData"))
            AssetDatabase.CreateFolder("Assets/Resources", "BossData");

        // Load boss sprites (first frame of each)
        Sprite bos01 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Png/Bos01/Bos01-animation_00.png");
        Sprite bos02 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Png/Bos02/Bos02-animation_00.png");
        Sprite bos03 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Png/Bos03/Bos03-animation_00.png");
        Sprite bos04 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Png/Bos04/Bos04-animation_00.png");
        Sprite bos05 = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Png/Bos05/Bos05-animation_00.png");

        // Load animator controllers (must run SetupBossAnimations first)
        var anim01 = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Bosses/Bos01.controller");
        var anim02 = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Bosses/Bos02.controller");
        var anim03 = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Bosses/Bos03.controller");
        var anim04 = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Bosses/Bos04.controller");
        var anim05 = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Bosses/Bos05.controller");

        // Boss 1 — Blob King (Wave 10) — Obstacles
        CreateBoss("BossData_BlobKing", "Blob King", 10, 15, 2, 20, 40,
            new Color(0.3f, 0.9f, 0.3f, 1f), bos01, anim01,
            BossPattern.Obstacle, "Bronze");

        // Boss 2 — Speedy (Wave 20) — Fast movement
        CreateBoss("BossData_Speedy", "Speedy", 20, 25, 3, 30, 50,
            new Color(0.4f, 0.3f, 0.9f, 1f), bos02, anim02,
            BossPattern.Speed, "Silver");

        // Boss 3 — Guardian (Wave 30) — Shield
        CreateBoss("BossData_Guardian", "Guardian", 30, 35, 3, 40, 60,
            new Color(0.2f, 0.8f, 0.4f, 1f), bos03, anim03,
            BossPattern.Shield, "Gold");

        // Boss 4 — Splitter (Wave 40) — Divides at 50% HP
        CreateBoss("BossData_Splitter", "Splitter", 40, 45, 4, 50, 80,
            new Color(0.9f, 0.3f, 0.6f, 1f), bos04, anim04,
            BossPattern.Split, "Diamond");

        // Boss 5 — Overlord (Wave 50) — All mechanics
        CreateBoss("BossData_Overlord", "Overlord", 50, 60, 5, 60, 100,
            new Color(0.6f, 0.4f, 0.2f, 1f), bos05, anim05,
            BossPattern.Overlord, "Legendary");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("5 BossData assets created in Resources/BossData/");
    }

    static void CreateBoss(string fileName, string bossName, int wave, int hp, int dmg,
        int coinMin, int coinMax, Color color, Sprite sprite, RuntimeAnimatorController anim,
        BossPattern pattern, string skinReward)
    {
        string path = $"Assets/Resources/BossData/{fileName}.asset";

        // Don't overwrite existing
        if (AssetDatabase.LoadAssetAtPath<BossData>(path) != null)
        {
            Debug.Log($"{fileName} already exists, skipping.");
            return;
        }

        BossData data = ScriptableObject.CreateInstance<BossData>();
        data.bossName = bossName;
        data.bossWave = wave;
        data.maxHP = hp;
        data.damage = dmg;
        data.coinDropMin = coinMin;
        data.coinDropMax = coinMax;
        data.color = color;
        data.sprite = sprite;
        data.animController = anim;
        data.pattern = pattern;
        data.skinRewardId = skinReward;

        AssetDatabase.CreateAsset(data, path);
    }
}
