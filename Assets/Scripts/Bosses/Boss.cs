using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    private BossData _bossData;
    private Enemy _enemy;
    private EnemyHealth _health;
    private bool _hasSplit;
    private bool _shielded;
    private float _shieldTimer;
    private float _moveDir = 1f;
    private float _moveSpeed;
    private List<GameObject> _obstacles = new List<GameObject>();
    private SpriteRenderer _shieldVisual;

    // Shield cycle: 3s shielded, 3s vulnerable
    private const float ShieldCycleDuration = 3f;
    // Movement
    private const float BaseMoveSpeed = 2f;

    public BossData BossInfo => _bossData;
    public bool IsShielded => _shielded;

    public void Init(BossData data)
    {
        _bossData = data;
        _enemy = GetComponent<Enemy>();
        _health = GetComponent<EnemyHealth>();

        _health.OnHPChanged += OnBossHPChanged;

        switch (data.pattern)
        {
            case BossPattern.Obstacle:
                SpawnObstacles(3);
                break;
            case BossPattern.Speed:
                _moveSpeed = BaseMoveSpeed;
                break;
            case BossPattern.Shield:
                StartShieldCycle();
                break;
            case BossPattern.Split:
                // split handled in OnBossHPChanged
                break;
            case BossPattern.Overlord:
                _moveSpeed = BaseMoveSpeed * 0.7f;
                StartShieldCycle();
                break;
        }
    }

    void Update()
    {
        if (_health == null || _health.IsDead) return;

        // Movement for Speed and Overlord patterns
        if (_moveSpeed > 0)
            UpdateMovement();

        // Shield timer
        if (_shieldVisual != null)
            UpdateShieldCycle();
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.OnHPChanged -= OnBossHPChanged;

        // Clean up obstacles
        foreach (var obs in _obstacles)
        {
            if (obs != null) Destroy(obs);
        }
    }

    // --- MOVEMENT PATTERN ---

    void UpdateMovement()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float halfW = cam.orthographicSize * cam.aspect - 0.8f;
        Vector3 pos = transform.position;
        pos.x += _moveDir * _moveSpeed * Time.deltaTime;

        if (pos.x > halfW) { pos.x = halfW; _moveDir = -1f; }
        else if (pos.x < -halfW) { pos.x = -halfW; _moveDir = 1f; }

        transform.position = pos;
    }

    // --- SHIELD PATTERN ---

    void StartShieldCycle()
    {
        // Create shield visual as child
        GameObject shieldGO = new GameObject("BossShield");
        shieldGO.transform.SetParent(transform);
        shieldGO.transform.localPosition = Vector3.zero;

        _shieldVisual = shieldGO.AddComponent<SpriteRenderer>();
        _shieldVisual.color = new Color(0.3f, 0.8f, 1f, 0.4f);
        _shieldVisual.sortingOrder = 10;

        // Use a simple circle sprite from resources or create programmatically
        _shieldVisual.sprite = CreateShieldSprite();
        shieldGO.transform.localScale = Vector3.one * 2f;

        _shieldTimer = 0f;
        _shielded = true;
    }

    void UpdateShieldCycle()
    {
        _shieldTimer += Time.deltaTime;
        if (_shieldTimer >= ShieldCycleDuration)
        {
            _shieldTimer = 0f;
            _shielded = !_shielded;
            _shieldVisual.enabled = _shielded;

            // Flash when shield drops
            if (!_shielded)
            {
                var flash = GetComponent<EnemyFlash>();
                if (flash != null) flash.Flash();
            }
        }

        // Pulse effect when shielded
        if (_shielded && _shieldVisual != null)
        {
            float pulse = 1.8f + Mathf.Sin(Time.time * 3f) * 0.2f;
            _shieldVisual.transform.localScale = Vector3.one * pulse;
            float alpha = 0.3f + Mathf.Sin(Time.time * 2f) * 0.1f;
            _shieldVisual.color = new Color(0.3f, 0.8f, 1f, alpha);
        }
    }

    Sprite CreateShieldSprite()
    {
        // Create a simple circular texture for shield
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;
        Color clear = new Color(1, 1, 1, 0);
        float center = size / 2f;
        float radius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist < radius - 2 && dist > radius - 6)
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f);
    }

    // Intercept damage when shielded
    public bool TryBlockDamage()
    {
        return _shielded;
    }

    // --- OBSTACLE PATTERN ---

    void SpawnObstacles(int count)
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float halfW = cam.orthographicSize * cam.aspect - 1f;
        float maxY = cam.orthographicSize - 1.5f;

        for (int i = 0; i < count; i++)
        {
            GameObject obs = new GameObject($"BossObstacle_{i}");
            obs.layer = LayerMask.NameToLayer("Wall");

            float x = Random.Range(-halfW * 0.8f, halfW * 0.8f);
            float y = Random.Range(0.5f, maxY * 0.8f);
            obs.transform.position = new Vector3(x, y, 0);

            // Add box collider for arrow bouncing
            BoxCollider2D col = obs.AddComponent<BoxCollider2D>();
            float scaleX = Random.Range(1f, 2f);
            float scaleY = Random.Range(0.5f, 1f);
            obs.transform.localScale = new Vector3(scaleX, scaleY, 1f);

            // Visual
            SpriteRenderer sr = obs.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.5f, 0.5f, 0.55f, 1f);
            sr.sortingOrder = -1;
            sr.sprite = CreateObstacleSprite();

            // Bouncy material like walls
            var mat = new PhysicsMaterial2D("ObstacleBounce");
            mat.bounciness = 1f;
            mat.friction = 0f;
            col.sharedMaterial = mat;

            _obstacles.Add(obs);
        }
    }

    Sprite CreateObstacleSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Point;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }

    // --- SPLIT PATTERN ---

    void OnBossHPChanged(int current, int max)
    {
        if (_hasSplit) return;
        if (_bossData == null) return;

        bool canSplit = _bossData.pattern == BossPattern.Split || _bossData.pattern == BossPattern.Overlord;
        if (!canSplit) return;

        // Split at 50% HP
        if (current <= max / 2 && current > 0)
        {
            _hasSplit = true;
            StartCoroutine(SplitRoutine(current, max));
        }
    }

    IEnumerator SplitRoutine(int currentHP, int maxHP)
    {
        // Brief pause for dramatic effect
        yield return new WaitForSeconds(0.3f);

        var spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner == null) yield break;

        // Spawn 2 mini versions
        int miniHP = Mathf.Max(1, currentHP / 2);
        int miniDamage = Mathf.Max(1, _bossData.damage / 2);

        for (int i = 0; i < 2; i++)
        {
            float offsetX = (i == 0) ? -1f : 1f;
            Vector3 pos = transform.position + new Vector3(offsetX, 0, 0);

            // Create a temporary EnemyData for mini-boss
            EnemyData miniData = ScriptableObject.CreateInstance<EnemyData>();
            miniData.enemyName = $"Mini_{_bossData.bossName}";
            miniData.maxHP = miniHP;
            miniData.damage = miniDamage;
            miniData.color = _bossData.color;
            miniData.sprite = _bossData.sprite;
            miniData.animController = _bossData.animController;
            miniData.coinDropMin = _bossData.coinDropMin / 4;
            miniData.coinDropMax = _bossData.coinDropMax / 4;

            // Use the spawner's enemy prefab
            var prefabField = typeof(EnemySpawner).GetField("enemyPrefab",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prefabField == null) continue;

            GameObject prefab = prefabField.GetValue(spawner) as GameObject;
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, pos, Quaternion.identity);
            go.layer = LayerMask.NameToLayer("Enemy");
            go.transform.localScale = transform.localScale * 0.6f;

            Enemy miniEnemy = go.GetComponent<Enemy>();
            miniEnemy.Init(miniData);

            // Register with spawner
            miniEnemy.Health.OnDied += (e) =>
            {
                spawner.GetType()
                    .GetMethod("HandleEnemyDied", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(spawner, new object[] { e });
            };
            spawner.ActiveEnemies.Add(miniEnemy);
        }

        Debug.Log($"Boss {_bossData.bossName} split into 2 mini-bosses!");
    }
}
