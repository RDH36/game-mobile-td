using UnityEngine;

[System.Serializable]
public class WaveEntry
{
    public EnemyData enemyData;
    public int count;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "ArrowStrike/WaveData")]
public class WaveData : ScriptableObject
{
    public string waveName;
    public WaveEntry[] entries;

    public int TotalEnemies
    {
        get
        {
            int total = 0;
            foreach (var e in entries) total += e.count;
            return total;
        }
    }
}
