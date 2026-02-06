using UnityEngine;

/// <summary>
/// Visual projectile fired by enemies during counter-attack.
/// Flies from enemy to bow, deals damage on arrival.
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    private Vector3 _target;
    private float _speed;
    private int _damage;
    private System.Action _onHit;
    private bool _hit;

    public void Init(Vector3 target, int damage, float speed, System.Action onHit)
    {
        _target = target;
        _damage = damage;
        _speed = speed;
        _onHit = onHit;
    }

    void Update()
    {
        if (_hit) return;

        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);

        // Rotate to face movement direction
        Vector3 dir = (_target - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        if (Vector3.Distance(transform.position, _target) < 0.1f)
        {
            _hit = true;
            _onHit?.Invoke();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Creates an enemy projectile at runtime (no prefab needed).
    /// </summary>
    public static EnemyProjectile Create(Vector3 from, Vector3 to, int damage, float speed, System.Action onHit)
    {
        GameObject go = new GameObject("EnemyProjectile");
        go.transform.position = from;

        // Create sprite
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 50;
        sr.color = new Color(1f, 0.3f, 0.1f, 1f); // orange-red

        Texture2D tex = new Texture2D(4, 8);
        Color[] pixels = new Color[32];
        for (int i = 0; i < 32; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 8), new Vector2(0.5f, 0.5f), 16f);

        go.transform.localScale = new Vector3(0.3f, 0.5f, 1f);

        EnemyProjectile proj = go.AddComponent<EnemyProjectile>();
        proj.Init(to, damage, speed, onHit);
        return proj;
    }
}
