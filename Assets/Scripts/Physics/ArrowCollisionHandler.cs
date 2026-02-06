using UnityEngine;

[RequireComponent(typeof(ArrowDurability))]
public class ArrowCollisionHandler : MonoBehaviour
{
    [SerializeField] private int damagePerHit = 1;

    private ArrowDurability _durability;

    public int DamagePerHit => damagePerHit;

    void Awake()
    {
        _durability = GetComponent<ArrowDurability>();
    }

    public void SetDamage(int damage)
    {
        damagePerHit = damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layer = collision.gameObject.layer;

        // Hit enemy — deal damage + reduce durability
        if (layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth health = collision.gameObject.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damagePerHit);
            }

            // Flash effect
            EnemyFlash flash = collision.gameObject.GetComponent<EnemyFlash>();
            if (flash != null)
            {
                flash.Flash();
            }

            _durability.TakeHit();
        }
    }

    void Update()
    {
        // Arrow fell below arena — destroy it
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }
}
