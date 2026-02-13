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
    private Coroutine _obstacleRoutine;
    private Sprite[] _bulletSprites;
    private const int MaxObstacles = 3;
    private const float ObstacleSpawnInterval = 1.5f;
    private const float ObstacleAttackInterval = 4f;
    private const float OrbitRadius = 1.2f;
    private const float OrbitSpeed = 120f; // degrees per second
    private float _orbitAngle;
    private bool _launching;

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

        // Load bullet sprites for orbit balls
        _bulletSprites = Resources.LoadAll<Sprite>("Sprites/Bullets");

        switch (data.pattern)
        {
            case BossPattern.Obstacle:
                _obstacleRoutine = StartCoroutine(ObstacleSpawnLoop());
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

        // Orbit cubes around boss
        if (_obstacles.Count > 0 && !_launching)
            UpdateOrbit();
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.OnHPChanged -= OnBossHPChanged;

        if (_obstacleRoutine != null)
            StopCoroutine(_obstacleRoutine);

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

    IEnumerator ObstacleSpawnLoop()
    {
        while (_health != null && !_health.IsDead)
        {
            // Spawn phase: fill up to MaxObstacles
            while (_obstacles.Count < MaxObstacles && _health != null && !_health.IsDead)
            {
                SpawnOrbitCube();
                yield return new WaitForSeconds(ObstacleSpawnInterval);
            }

            // Wait before attacking
            yield return new WaitForSeconds(ObstacleAttackInterval);

            // Attack phase: launch all cubes at the player
            if (_health != null && !_health.IsDead)
                yield return LaunchCubesAtPlayer();
        }
    }

    void SpawnOrbitCube()
    {
        // If at max, remove oldest
        if (_obstacles.Count >= MaxObstacles)
        {
            GameObject oldest = _obstacles[0];
            _obstacles.RemoveAt(0);
            if (oldest != null) Destroy(oldest);
        }

        GameObject cube = new GameObject("BossCube");
        cube.layer = LayerMask.NameToLayer("Wall");

        // Start at orbit position around boss
        cube.transform.position = transform.position;

        // Bullet size
        cube.transform.localScale = Vector3.one * 0.6f;

        // Circle collider for arrow bouncing
        CircleCollider2D col = cube.AddComponent<CircleCollider2D>();
        var mat = new PhysicsMaterial2D("BallBounce");
        mat.bounciness = 1f;
        mat.friction = 0f;
        col.sharedMaterial = mat;

        // Visual — random bullet sprite
        SpriteRenderer sr = cube.AddComponent<SpriteRenderer>();
        if (_bulletSprites != null && _bulletSprites.Length > 0)
            sr.sprite = _bulletSprites[Random.Range(0, _bulletSprites.Length)];
        else
            sr.sprite = CreateBallSprite();
        sr.sortingOrder = 5;

        _obstacles.Add(cube);
    }

    void UpdateOrbit()
    {
        _orbitAngle += OrbitSpeed * Time.deltaTime;

        for (int i = 0; i < _obstacles.Count; i++)
        {
            if (_obstacles[i] == null) continue;

            float angle = (_orbitAngle + i * (360f / Mathf.Max(1, _obstacles.Count))) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * OrbitRadius, Mathf.Sin(angle) * OrbitRadius, 0f);
            _obstacles[i].transform.position = transform.position + offset;

            // Spin the cube itself
            _obstacles[i].transform.Rotate(0, 0, 200f * Time.deltaTime);
        }
    }

    Sprite CreateBallSprite()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;
        float center = size / 2f;
        float radius = size / 2f;
        Color clear = new Color(1, 1, 1, 0);

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                tex.SetPixel(x, y, dist < radius ? Color.white : clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
    }

    IEnumerator LaunchCubesAtPlayer()
    {
        _launching = true;

        BowHealth bow = FindFirstObjectByType<BowHealth>();
        if (bow == null) { _launching = false; yield break; }

        Vector3 target = bow.transform.position;
        float launchSpeed = 8f;

        // Snapshot cubes to launch
        var cubes = new List<GameObject>(_obstacles);
        _obstacles.Clear();

        foreach (var cube in cubes)
        {
            if (cube == null) continue;

            // Disable collider so it doesn't bounce arrows during flight
            var col = cube.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            StartCoroutine(MoveCubeToTarget(cube, target, launchSpeed, bow));
        }

        // Wait for all cubes to arrive
        yield return new WaitForSeconds(1.5f);

        _launching = false;
    }

    IEnumerator MoveCubeToTarget(GameObject cube, Vector3 target, float speed, BowHealth bow)
    {
        while (cube != null && Vector3.Distance(cube.transform.position, target) > 0.3f)
        {
            cube.transform.position = Vector3.MoveTowards(cube.transform.position, target, speed * Time.deltaTime);
            cube.transform.Rotate(0, 0, 400f * Time.deltaTime);
            yield return null;
        }

        if (cube != null)
        {
            // Deal 1 damage to bow
            if (bow != null && bow.CurrentHP > 0)
            {
                bow.TakeDamage(1);
                DamagePopup.Create(bow.transform.position, 1);
                if (CameraShake.Instance != null)
                    CameraShake.Instance.Shake(0.15f, 0.15f);
            }

            Destroy(cube);
        }
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
