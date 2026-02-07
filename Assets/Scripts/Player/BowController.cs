using UnityEngine;
using MoreMountains.Feedbacks;

public class BowController : MonoBehaviour
{
    [SerializeField] private float maxLaunchSpeed = 15f;
    [SerializeField] private float minLaunchSpeed = 5f;
    [SerializeField] private float squashIntensity = 0.5f;

    private SwipeDetector _swipe;
    private Vector3 _baseScale;
    private MMSpringScale _spring;

    public Vector2 LaunchDirection => _swipe != null ? _swipe.SwipeDirection : Vector2.up;
    public float LaunchSpeed => _swipe != null
        ? Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, _swipe.SwipePower)
        : 0f;
    public bool IsAiming => _swipe != null && _swipe.IsSwiping && _swipe.SwipePower > 0f;

    public event System.Action<Vector2, float> OnShoot; // direction, speed

    void Awake()
    {
        _swipe = GetComponent<SwipeDetector>();
        if (_swipe == null)
            _swipe = GetComponentInParent<SwipeDetector>();
        _baseScale = transform.localScale;

        // Feel: spring-based scale for recoil snap-back
        _spring = gameObject.AddComponent<MMSpringScale>();
    }

    void OnEnable()
    {
        if (_swipe != null)
        {
            _swipe.OnSwipeUpdated += HandleSwipeUpdated;
            _swipe.OnSwipeReleased += HandleSwipeReleased;
        }
    }

    void OnDisable()
    {
        if (_swipe != null)
        {
            _swipe.OnSwipeUpdated -= HandleSwipeUpdated;
            _swipe.OnSwipeReleased -= HandleSwipeReleased;
        }
    }

    private void HandleSwipeUpdated()
    {
        if (_swipe.SwipePower <= 0f) return;

        // Rotate bow to face the swipe direction
        float angle = Mathf.Atan2(_swipe.SwipeDirection.y, _swipe.SwipeDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // Squash & stretch during aiming — instant, no spring
        float s = squashIntensity * _swipe.SwipePower;
        Vector3 squashed = new Vector3(
            _baseScale.x * (1f + s),
            _baseScale.y * (1f - s),
            _baseScale.z
        );
        _spring.MoveToInstant(squashed);
    }

    private void HandleSwipeReleased(Vector2 direction, float power)
    {
        // Feel: spring back to base scale + recoil kick
        _spring.MoveTo(_baseScale);
        float kick = squashIntensity * power;
        _spring.Bump(new Vector3(-kick, kick, 0f));

        float speed = Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, power);
        OnShoot?.Invoke(direction, speed);
    }
}
