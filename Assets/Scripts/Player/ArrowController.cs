using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private TrailRenderer _trail;

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
        var col = GetComponent<Collider2D>();
        if (col != null) col.sharedMaterial = bounceMat;

        // Setup trail
        _trail = gameObject.AddComponent<TrailRenderer>();
        _trail.time = 0.12f;
        _trail.startWidth = 0.12f;
        _trail.endWidth = 0f;
        _trail.startColor = new Color(1f, 0.9f, 0.3f, 0.8f);
        _trail.endColor = new Color(1f, 0.5f, 0.1f, 0f);
        _trail.material = new Material(Shader.Find("Sprites/Default"));
        _trail.sortingOrder = 2;
        _trail.numCornerVertices = 2;
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
