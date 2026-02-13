using UnityEditor;
using UnityEngine;

public static class UpdateBossHP
{
    public static string Execute()
    {
        string[] bossFiles = new string[]
        {
            "Assets/Resources/BossData/BossData_BlobKing.asset",
            "Assets/Resources/BossData/BossData_Speedy.asset",
            "Assets/Resources/BossData/BossData_Guardian.asset",
            "Assets/Resources/BossData/BossData_Splitter.asset",
            "Assets/Resources/BossData/BossData_Overlord.asset"
        };

        // New HP values scaled to player power at each stage
        // Player needs to use most of their arrows to beat the boss
        int[] newHP =      { 60,  150,  250,  400,  700 };
        int[] newDamage =  {  2,    3,    4,    5,    7 };

        string result = "Boss HP updated:\n";

        for (int i = 0; i < bossFiles.Length; i++)
        {
            BossData boss = AssetDatabase.LoadAssetAtPath<BossData>(bossFiles[i]);
            if (boss == null) { result += $"  NOT FOUND: {bossFiles[i]}\n"; continue; }

            int oldHP = boss.maxHP;
            boss.maxHP = newHP[i];
            boss.damage = newDamage[i];
            EditorUtility.SetDirty(boss);

            result += $"  {boss.bossName} (W{boss.bossWave}): HP {oldHP} -> {newHP[i]}, Dmg {newDamage[i]}\n";
        }

        AssetDatabase.SaveAssets();
        return result;
    }
}
