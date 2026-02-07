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

        // Hit wall — perturb angle to break loops + spawn particles
        if (layer == LayerMask.NameToLayer("Wall"))
        {
            ContactPoint2D contact = collision.GetContact(0);
            SpawnWallHitParticles(contact.point, contact.normal);

            // Slight random angle perturbation to prevent infinite bounce loops
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 vel = rb.linearVelocity;
                float angle = Random.Range(-3f, 3f);
                float rad = angle * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                rb.linearVelocity = new Vector2(vel.x * cos - vel.y * sin, vel.x * sin + vel.y * cos);
            }
        }

        // Hit enemy — deal damage + reduce durability
        if (layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth health = collision.gameObject.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damagePerHit);
                bool killed = health.IsDead;
                CameraShake.Instance?.Shake(killed ? 0.3f : 0.12f, killed ? 0.25f : 0.1f);

                // Floating damage number
                DamagePopup.Create(collision.transform.position, damagePerHit);

                // Knockback if still alive
                if (!killed)
                {
                    Vector2 knockDir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
                    collision.transform.position += (Vector3)(knockDir * 0.3f);
                }
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

    void SpawnWallHitParticles(Vector2 position, Vector2 normal)
    {
        GameObject go = new GameObject("WallHitFX");
        go.transform.position = position;

        // Orient particles along the wall normal (away from wall)
        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0, 0, angle);

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.duration = 0.3f;
        main.loop = false;
        main.startLifetime = 0.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.06f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.5f, 0.5f, 0.5f, 1f),
            new Color(0.7f, 0.7f, 0.7f, 1f)
        );
        main.gravityModifier = 1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 5, 8) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35f;
        shape.radius = 0.05f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(new Color(0.8f, 0.8f, 0.8f), 0f), new GradientColorKey(new Color(0.4f, 0.4f, 0.4f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = grad;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.sortingOrder = 5;

        ps.Play();
        Destroy(go, 2f);
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
