using UnityEditor;
using UnityEngine;

public static class CreateWaveAssets
{
    [MenuItem("Tools/Create Wave Assets")]
    public static void Execute()
    {
        // Ensure folders exist
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/WaveData"))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "WaveData");

        // Load enemy data
        EnemyData weak = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/ScriptableObjects/EnemyData/EnemyData_Weak.asset");
        EnemyData medium = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/ScriptableObjects/EnemyData/EnemyData_Medium.asset");
        EnemyData strong = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/ScriptableObjects/EnemyData/EnemyData_Strong.asset");

        if (strong == null)
        {
            strong = ScriptableObject.CreateInstance<EnemyData>();
            strong.enemyName = "Strong";
            strong.maxHP = 3;
            strong.damage = 3;
            strong.color = new Color(0.9f, 0.2f, 0.2f, 1f);
            strong.coinDropMin = 5;
            strong.coinDropMax = 10;
            AssetDatabase.CreateAsset(strong, "Assets/ScriptableObjects/EnemyData/EnemyData_Strong.asset");
        }

        // Delete old wave assets
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/WaveData/Wave_01.asset");
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/WaveData/Wave_02.asset");
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/WaveData/Wave_03.asset");
        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/WaveData/Wave_04.asset");

        // Wave 1: 5 weak only
        WaveData w1 = ScriptableObject.CreateInstance<WaveData>();
        w1.waveName = "Scouts";
        w1.entries = new WaveEntry[]
        {
            new WaveEntry { enemyData = weak, count = 5 }
        };
        AssetDatabase.CreateAsset(w1, "Assets/ScriptableObjects/WaveData/Wave_01.asset");

        // Wave 2: 5 weak + 1 medium
        WaveData w2 = ScriptableObject.CreateInstance<WaveData>();
        w2.waveName = "Patrol";
        w2.entries = new WaveEntry[]
        {
            new WaveEntry { enemyData = weak, count = 5 },
            new WaveEntry { enemyData = medium, count = 1 }
        };
        AssetDatabase.CreateAsset(w2, "Assets/ScriptableObjects/WaveData/Wave_02.asset");

        // Wave 3: 3 weak + 4 medium
        WaveData w3 = ScriptableObject.CreateInstance<WaveData>();
        w3.waveName = "Assault";
        w3.entries = new WaveEntry[]
        {
            new WaveEntry { enemyData = weak, count = 3 },
            new WaveEntry { enemyData = medium, count = 4 }
        };
        AssetDatabase.CreateAsset(w3, "Assets/ScriptableObjects/WaveData/Wave_03.asset");

        // Wave 4: 2 weak + 5 medium + 2 strong
        WaveData w4 = ScriptableObject.CreateInstance<WaveData>();
        w4.waveName = "Final Stand";
        w4.entries = new WaveEntry[]
        {
            new WaveEntry { enemyData = weak, count = 2 },
            new WaveEntry { enemyData = medium, count = 5 },
            new WaveEntry { enemyData = strong, count = 2 }
        };
        AssetDatabase.CreateAsset(w4, "Assets/ScriptableObjects/WaveData/Wave_04.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("4 WaveData assets recreated with precise enemy counts!");
    }
}
