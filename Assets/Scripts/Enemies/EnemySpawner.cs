using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private float spawnPadding = 0.5f;
    [SerializeField] private float minSpacingBetweenEnemies = 1.2f;

    private List<Enemy> _activeEnemies = new List<Enemy>();

    public List<Enemy> ActiveEnemies => _activeEnemies;
    public int AliveCount
    {
        get
        {
            _activeEnemies.RemoveAll(e => e == null || e.Health == null || e.Health.IsDead);
            return _activeEnemies.Count;
        }
    }

    public event System.Action<Enemy> OnEnemyKilled;
    public event System.Action OnAllEnemiesKilled;

    public void SpawnBoss(BossData bossData) => SpawnBossWave(bossData, null);

    public void SpawnBossWave(BossData bossData, WaveEntry[] guards)
    {
        ClearEnemies();

        Camera cam = Camera.main;
        float topY = cam.orthographicSize - 1.5f;

        // Boss target position (near the top)
        Vector3 bossPos = new Vector3(0f, topY - 0.5f, 0f);
        // Start off-screen above the top wall
        Vector3 startPos = new Vector3(0f, cam.orthographicSize + 2f, 0f);

        GameObject bPrefab = bossPrefab != null ? bossPrefab : enemyPrefab;
        GameObject go = Instantiate(bPrefab, startPos, Quaternion.identity);
        go.layer = LayerMask.NameToLayer("Enemy");

        // Create a temporary EnemyData from BossData for Enemy.Init compatibility
        EnemyData tempData = ScriptableObject.CreateInstance<EnemyData>();
        tempData.enemyName = bossData.bossName;
        tempData.maxHP = bossData.maxHP;
        tempData.damage = bossData.damage;
        tempData.color = Color.white;
        tempData.sprite = bossData.sprite;
        tempData.animController = bossData.animController;
        tempData.coinDropMin = bossData.coinDropMin;
        tempData.coinDropMax = bossData.coinDropMax;

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Init(tempData);

        // Add Boss behavior component
        Boss boss = go.AddComponent<Boss>();
        boss.Init(bossData);

        enemy.Health.OnDied += HandleEnemyDied;
        _activeEnemies.Add(enemy);

        go.name = $"Boss_{bossData.bossName}";

        // Wait for boss name banner to disappear (2s), then slide boss in
        StartCoroutine(BossEntrance(go.transform, bossPos, 2.2f));

        // Spawn guard monsters around the boss
        if (guards != null && guards.Length > 0)
            SpawnGuards(guards, bossPos);
    }

    void SpawnGuards(WaveEntry[] guards, Vector3 bossPos)
    {
        Camera cam = Camera.main;
        Rect safe = Screen.safeArea;
        Vector2 safeMin = cam.ScreenToWorldPoint(new Vector2(safe.xMin, safe.yMin));
        Vector2 safeMax = cam.ScreenToWorldPoint(new Vector2(safe.xMax, safe.yMax));

        // Guards spawn IN FRONT of the boss (below it) to protect it
        // Keep 1.5 units clearance from boss center to avoid overlap
        float arenaMinX = safeMin.x + spawnPadding;
        float arenaMaxX = safeMax.x - spawnPadding;
        float guardMaxY = bossPos.y - 1.5f;
        float guardMinY = Mathf.Max(-1f, guardMaxY - 4f);

        // Reserve boss position so guards never spawn on top of it
        List<Vector2> usedPositions = new List<Vector2> { bossPos };

        foreach (WaveEntry entry in guards)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Vector2 pos = FindValidPosition(arenaMinX, arenaMaxX, guardMinY, guardMaxY, usedPositions, 0.9f);
                usedPositions.Add(pos);

                GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);
                go.layer = LayerMask.NameToLayer("Enemy");

                Enemy guardEnemy = go.GetComponent<Enemy>();
                guardEnemy.Init(entry.enemyData);

                guardEnemy.Health.OnDied += HandleEnemyDied;
                _activeEnemies.Add(guardEnemy);
            }
        }
    }

    System.Collections.IEnumerator BossEntrance(Transform bossT, Vector3 target, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        float duration = 1.2f;
        float elapsed = 0f;
        Vector3 start = bossT.position;

        while (elapsed < duration && bossT != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            bossT.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        if (bossT != null)
            bossT.position = target;
    }

    public void SpawnWave(WaveEntry[] entries) => SpawnWave(entries, 1f, 1f, 0);

    public void SpawnWave(WaveEntry[] entries, float hpMultiplier, float coinMultiplier, int waveNum = 0)
    {
        ClearEnemies();

        Camera cam = Camera.main;
        Rect safe = Screen.safeArea;
        Vector2 safeMin = cam.ScreenToWorldPoint(new Vector2(safe.xMin, safe.yMin));
        Vector2 safeMax = cam.ScreenToWorldPoint(new Vector2(safe.xMax, safe.yMax));

        // Spawn only in upper half of arena (above center)
        float arenaMinX = safeMin.x + spawnPadding;
        float arenaMaxX = safeMax.x - spawnPadding;
        float arenaMinY = 0f; // center of screen
        float arenaMaxY = safeMax.y - spawnPadding - 0.5f; // below top wall

        // Calculate total enemies and adapt spacing
        int totalEnemies = 0;
        foreach (var e in entries) totalEnemies += e.count;

        float areaW = arenaMaxX - arenaMinX;
        float areaH = arenaMaxY - arenaMinY;
        float area = areaW * areaH;

        // Reduce spacing if too many enemies for the area
        float spacing = Mathf.Min(minSpacingBetweenEnemies, Mathf.Sqrt(area / totalEnemies) * 0.8f);
        spacing = Mathf.Max(spacing, 0.5f); // never less than 0.5

        List<Vector2> usedPositions = new List<Vector2>();

        foreach (WaveEntry entry in entries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Vector2 pos = FindValidPosition(arenaMinX, arenaMaxX, arenaMinY, arenaMaxY, usedPositions, spacing);
                usedPositions.Add(pos);

                GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);
                go.layer = LayerMask.NameToLayer("Enemy");

                Enemy enemy = go.GetComponent<Enemy>();
                enemy.Init(entry.enemyData, hpMultiplier, coinMultiplier);

                // After wave 10, some enemies get an orbiting ball
                float orbitChance = InfiniteWaveGenerator.GetOrbitBallChance(waveNum);
                if (orbitChance > 0f && Random.value < orbitChance)
                    go.AddComponent<EnemyOrbitBall>();

                enemy.Health.OnDied += HandleEnemyDied;
                _activeEnemies.Add(enemy);
            }
        }
    }

    Vector2 FindValidPosition(float minX, float maxX, float minY, float maxY, List<Vector2> used, float spacing)
    {
        for (int attempt = 0; attempt < 200; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            bool valid = true;
            foreach (Vector2 p in used)
            {
                if (Vector2.Distance(candidate, p) < spacing)
                {
                    valid = false;
                    break;
                }
            }

            if (valid) return candidate;
        }

        // Fallback: try again with halved spacing
        for (int attempt = 0; attempt < 100; attempt++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );

            bool valid = true;
            foreach (Vector2 p in used)
            {
                if (Vector2.Distance(candidate, p) < spacing * 0.5f)
                {
                    valid = false;
                    break;
                }
            }

            if (valid) return candidate;
        }

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    void HandleEnemyDied(Enemy enemy)
    {
        OnEnemyKilled?.Invoke(enemy);

        // Spawn death FX at enemy's scale
        var arrowMgr = FindFirstObjectByType<ArrowManager>();
        if (arrowMgr != null && arrowMgr.DeathFXPrefab != null)
        {
            GameObject fx = Instantiate(arrowMgr.DeathFXPrefab, enemy.transform.position, Quaternion.identity);
            fx.transform.localScale = enemy.transform.localScale;
        }

        // Destroy enemy after short delay for visual feedback
        Destroy(enemy.gameObject, 0.15f);

        // Check if all enemies dead (next frame to let destroy happen)
        StartCoroutine(CheckAllDead());
    }

    System.Collections.IEnumerator CheckAllDead()
    {
        yield return null;
        if (AliveCount <= 0)
        {
            OnAllEnemiesKilled?.Invoke();
        }
    }

    public void ClearEnemies()
    {
        foreach (var e in _activeEnemies)
        {
            if (e != null) Destroy(e.gameObject);
        }
        _activeEnemies.Clear();
    }
}
