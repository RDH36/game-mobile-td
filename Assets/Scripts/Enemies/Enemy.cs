using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    private EnemyData _data;
    private EnemyHealth _health;
    private SpriteRenderer _sr;
    private EnemyHealthBar _healthBar;
    private float _coinMultiplier = 1f;

    public EnemyData Data => _data;
    public EnemyHealth Health => _health;

    public void Init(EnemyData data, float hpMultiplier = 1f, float coinMultiplier = 1f)
    {
        _data = data;
        _coinMultiplier = coinMultiplier;
        _health = GetComponent<EnemyHealth>();
        _sr = GetComponent<SpriteRenderer>();

        _health.Init(Mathf.CeilToInt(data.maxHP * hpMultiplier));

        // Pick a random visual variant for variety
        var (chosenSprite, chosenAnim) = data.GetRandomVisual();
        if (chosenSprite != null) _sr.sprite = chosenSprite;
        _sr.color = data.color;

        // Auto-fit collider to sprite bounds
        FitColliderToSprite();

        gameObject.name = $"Enemy_{data.enemyName}";

        // Setup Animator for idle animation
        if (chosenAnim != null)
        {
            var animator = gameObject.GetComponent<Animator>();
            if (animator == null) animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = chosenAnim;
        }

        // Sprite-based health bar above enemy
        SetupHealthBar();

        // Listen for HP changes
        _health.OnHPChanged += OnHPChanged;
        _health.OnDied += OnDied;
    }

    void SetupHealthBar()
    {
        _healthBar = gameObject.AddComponent<EnemyHealthBar>();
        float spriteH = _sr.sprite != null ? _sr.sprite.bounds.size.y : 1f;
        _healthBar.Setup(spriteH);
    }

    void OnHPChanged(int current, int max)
    {
        if (_healthBar != null)
            _healthBar.UpdateBar(current, max);
    }

    void OnDied(Enemy enemy)
    {
        _health.OnHPChanged -= OnHPChanged;
        _health.OnDied -= OnDied;
    }

    void FitColliderToSprite()
    {
        if (_sr == null || _sr.sprite == null) return;

        Vector2 spriteSize = _sr.sprite.bounds.size;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.size = spriteSize;
            box.offset = Vector2.zero;
            return;
        }

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle != null)
        {
            circle.radius = Mathf.Max(spriteSize.x, spriteSize.y) * 0.5f;
            circle.offset = Vector2.zero;
        }
    }

    public int GetCoinDrop()
    {
        int baseDrop = Random.Range(_data.coinDropMin, _data.coinDropMax + 1);
        return Mathf.CeilToInt(baseDrop * _coinMultiplier);
    }
}
