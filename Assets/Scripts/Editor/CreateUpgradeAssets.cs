using UnityEditor;
using UnityEngine;

public static class CreateUpgradeAssets
{
    [MenuItem("Tools/Create Upgrade Assets")]
    public static void Execute()
    {
        // Create in Resources so they work in builds
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Upgrades"))
            AssetDatabase.CreateFolder("Assets/Resources", "Upgrades");

        // Clean old locations
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/UpgradeData/Upgrade_Durability.asset");
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/UpgradeData/Upgrade_ArrowCount.asset");
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/UpgradeData/Upgrade_Damage.asset");
        AssetDatabase.DeleteAsset("Assets/Resources/Upgrades/Upgrade_Durability.asset");
        AssetDatabase.DeleteAsset("Assets/Resources/Upgrades/Upgrade_ArrowCount.asset");
        AssetDatabase.DeleteAsset("Assets/Resources/Upgrades/Upgrade_Damage.asset");
        AssetDatabase.DeleteAsset("Assets/Resources/Upgrades/Upgrade_Speed.asset");
        AssetDatabase.DeleteAsset("Assets/Resources/Upgrades/Upgrade_Heal.asset");

        // +1 Durability (cost: 8)
        Create("Upgrade_Durability",
            "Fleche Robuste", "Chaque fleche traverse +1 ennemi avant de se briser",
            UpgradeType.ArrowDurability, 1, 8);

        // +1 Arrow (cost: 12)
        Create("Upgrade_ArrowCount",
            "Carquois Elargi", "+1 fleche supplementaire par vague",
            UpgradeType.ArrowCount, 1, 12);

        // +1 Damage (cost: 15)
        Create("Upgrade_Damage",
            "Pointe Aceree", "Les fleches infligent +1 degat par touche",
            UpgradeType.ArrowDamage, 1, 15);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Upgrade assets created in Resources/Upgrades/");
    }

    static void Create(string fileName, string name, string desc, UpgradeType type, int value, int cost)
    {
        UpgradeData data = ScriptableObject.CreateInstance<UpgradeData>();
        data.upgradeName = name;
        data.description = desc;
        data.upgradeType = type;
        data.value = value;
        data.cost = cost;
        AssetDatabase.CreateAsset(data, $"Assets/Resources/Upgrades/{fileName}.asset");
    }
}
