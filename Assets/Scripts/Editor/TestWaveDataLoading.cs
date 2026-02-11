using UnityEditor;
using UnityEngine;

public static class TestWaveDataLoading
{
    public static void Execute()
    {
        WaveData[] loaded = Resources.LoadAll<WaveData>("Waves");
        Debug.Log($"Resources.LoadAll<WaveData>(\"Waves\") found {loaded.Length} waves:");
        foreach (var w in loaded)
            Debug.Log($"  - {w.name}: {w.entries.Length} entries, {w.TotalEnemies} enemies");
    }
}
