using UnityEngine;

public class BowController : MonoBehaviour
{
    [SerializeField] private float maxLaunchSpeed = 15f;
    [SerializeField] private float minLaunchSpeed = 5f;

    private SwipeDetector _swipe;

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
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 because bow sprite points up
    }

    private void HandleSwipeReleased(Vector2 direction, float power)
    {
        float speed = Mathf.Lerp(minLaunchSpeed, maxLaunchSpeed, power);
        OnShoot?.Invoke(direction, speed);
    }
}
