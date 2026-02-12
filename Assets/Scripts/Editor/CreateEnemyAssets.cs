using UnityEditor;
using UnityEngine;

public static class CreateEnemyAssets
{
    [MenuItem("Tools/Create Enemy Assets")]
    public static void Execute()
    {
        // Enemy Weak
        EnemyData weak = ScriptableObject.CreateInstance<EnemyData>();
        weak.enemyName = "Weak";
        weak.maxHP = 1;
        weak.damage = 1;
        weak.color = new Color(0.2f, 0.8f, 0.3f, 1f); // green
        weak.coinDropMin = 1;
        weak.coinDropMax = 2;
        AssetDatabase.CreateAsset(weak, "Assets/ScriptableObjects/EnemyData/EnemyData_Weak.asset");

        // Enemy Medium
        EnemyData medium = ScriptableObject.CreateInstance<EnemyData>();
        medium.enemyName = "Medium";
        medium.maxHP = 2;
        medium.damage = 2;
        medium.color = new Color(0.2f, 0.4f, 0.9f, 1f); // blue
        medium.coinDropMin = 3;
        medium.coinDropMax = 5;
        AssetDatabase.CreateAsset(medium, "Assets/ScriptableObjects/EnemyData/EnemyData_Medium.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Enemy ScriptableObjects created!");
    }
}
