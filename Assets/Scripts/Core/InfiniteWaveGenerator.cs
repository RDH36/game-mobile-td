using UnityEngine;

public static class InfiniteWaveGenerator
{
    // Gentle curve: starts at ~8 (wave 5), grows slowly, caps at 25
    // Wave 5→8, 10→10, 15→12, 20→14, 30→18, 50→25
    public static int GetEnemyCount(int wave)
    {
        return Mathf.Min(6 + Mathf.FloorToInt(wave * 0.4f), 25);
    }

    public static float GetHPMultiplier(int wave)
    {
        return 1f + (wave * 0.08f);
    }

    public static float GetCoinMultiplier(int wave)
    {
        return 1f + (wave * 0.1f);
    }

    /// <summary>
    /// Generate wave composition based on wave number.
    /// Wave 1-14: Weak + Medium
    /// Wave 15-24: + Strong
    /// Wave 25+: + Elite
    /// </summary>
    public static WaveEntry[] Generate(int waveNumber, EnemyData weak, EnemyData medium, EnemyData strong, EnemyData elite)
    {
        int totalCount = GetEnemyCount(waveNumber);

        if (waveNumber >= 25 && elite != null)
            return DistributeFour(totalCount, weak, medium, strong, elite, waveNumber);
        if (waveNumber >= 15 && strong != null)
            return DistributeThree(totalCount, weak, medium, strong, waveNumber);
        return DistributeTwo(totalCount, weak, medium);
    }

    static WaveEntry[] DistributeTwo(int total, EnemyData weak, EnemyData medium)
    {
        int mediumCount = Mathf.RoundToInt(total * 0.4f);
        int weakCount = total - mediumCount;
        return MakeEntries(weak, weakCount, medium, mediumCount);
    }

    static WaveEntry[] DistributeThree(int total, EnemyData weak, EnemyData medium, EnemyData strong, int wave)
    {
        // Strong presence grows from 20% at wave 15 to 35% at wave 24
        float strongPct = Mathf.Lerp(0.2f, 0.35f, (wave - 15f) / 9f);
        int strongCount = Mathf.Max(1, Mathf.RoundToInt(total * strongPct));
        int remaining = total - strongCount;
        int mediumCount = Mathf.RoundToInt(remaining * 0.5f);
        int weakCount = remaining - mediumCount;
        return MakeEntries(weak, weakCount, medium, mediumCount, strong, strongCount);
    }

    static WaveEntry[] DistributeFour(int total, EnemyData weak, EnemyData medium, EnemyData strong, EnemyData elite, int wave)
    {
        // Elite grows from 15% at wave 25 to 30% at wave 50+
        float elitePct = Mathf.Lerp(0.15f, 0.30f, Mathf.Clamp01((wave - 25f) / 25f));
        float strongPct = 0.25f;
        int eliteCount = Mathf.Max(1, Mathf.RoundToInt(total * elitePct));
        int strongCount = Mathf.Max(1, Mathf.RoundToInt(total * strongPct));
        int remaining = total - eliteCount - strongCount;
        int mediumCount = Mathf.RoundToInt(remaining * 0.55f);
        int weakCount = remaining - mediumCount;
        weakCount = Mathf.Max(0, weakCount);
        return MakeEntries(weak, weakCount, medium, mediumCount, strong, strongCount, elite, eliteCount);
    }

    static WaveEntry[] MakeEntries(params object[] pairs)
    {
        int entryCount = 0;
        for (int i = 0; i < pairs.Length; i += 2)
        {
            int count = (int)pairs[i + 1];
            if (count > 0) entryCount++;
        }

        WaveEntry[] entries = new WaveEntry[entryCount];
        int idx = 0;
        for (int i = 0; i < pairs.Length; i += 2)
        {
            int count = (int)pairs[i + 1];
            if (count > 0)
            {
                entries[idx] = new WaveEntry
                {
                    enemyData = (EnemyData)pairs[i],
                    count = count
                };
                idx++;
            }
        }
        return entries;
    }
}
