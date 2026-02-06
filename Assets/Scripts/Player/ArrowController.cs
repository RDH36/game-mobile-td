using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowController : MonoBehaviour
{
    private Rigidbody2D _rb;

    public Vector2 Velocity => _rb != null ? _rb.linearVelocity : Vector2.zero;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Apply bounce material in code (perfect bounce, no friction)
        var bounceMat = new PhysicsMaterial2D("ArrowBounce")
        {
            bounciness = 1f,
            friction = 0f
        };
        _rb.sharedMaterial = bounceMat;
        // Also set on collider
        var col = GetComponent<Collider2D>();
        if (col != null) col.sharedMaterial = bounceMat;
    }

    public void Launch(Vector2 direction, float speed)
    {
        _rb.linearVelocity = direction.normalized * speed;
    }

    void Update()
    {
        // Rotate arrow to face movement direction
        if (_rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(_rb.linearVelocity.y, _rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}
