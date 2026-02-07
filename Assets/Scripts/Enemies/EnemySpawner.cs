using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
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

    public void SpawnWave(WaveEntry[] entries)
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
                enemy.Init(entry.enemyData);

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
