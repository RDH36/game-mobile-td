using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetector : MonoBehaviour
{
    [SerializeField] private float minSwipeDistance = 0.5f;
    [SerializeField] private float maxSwipeDistance = 3f;

    private InputAction _touchPosition;
    private InputAction _touchPress;

    private Vector2 _startScreenPos;
    private bool _isSwiping;
    private Camera _cam;

    public bool IsSwiping => _isSwiping;
    public Vector2 SwipeDirection { get; private set; }
    public float SwipePower { get; private set; }
    public Vector2 StartWorldPos { get; private set; }
    public Vector2 CurrentWorldPos { get; private set; }

    public event System.Action OnSwipeStarted;
    public event System.Action OnSwipeUpdated;
    public event System.Action<Vector2, float> OnSwipeReleased;

    void Awake()
    {
        _cam = Camera.main;
    }

    void OnEnable()
    {
        _touchPosition = new InputAction("TouchPosition", InputActionType.Value, "<Touchscreen>/position");
        _touchPosition.AddBinding("<Mouse>/position");

        _touchPress = new InputAction("TouchPress", InputActionType.Button, "<Touchscreen>/Press");
        _touchPress.AddBinding("<Mouse>/leftButton");

        _touchPress.started += OnTouchStarted;
        _touchPress.canceled += OnTouchReleased;

        _touchPosition.Enable();
        _touchPress.Enable();
    }

    void OnDisable()
    {
        if (_touchPress != null)
        {
            _touchPress.started -= OnTouchStarted;
            _touchPress.canceled -= OnTouchReleased;
            _touchPress.Disable();
            _touchPress.Dispose();
        }
        if (_touchPosition != null)
        {
            _touchPosition.Disable();
            _touchPosition.Dispose();
        }
    }

    void Update()
    {
        if (!_isSwiping || _touchPosition == null) return;

        Vector2 screenPos = _touchPosition.ReadValue<Vector2>();
        CurrentWorldPos = _cam.ScreenToWorldPoint(screenPos);

        Vector2 delta = StartWorldPos - CurrentWorldPos;
        float distance = delta.magnitude;

        if (distance < minSwipeDistance)
        {
            SwipeDirection = Vector2.zero;
            SwipePower = 0f;
        }
        else
        {
            SwipeDirection = delta.normalized;
            SwipePower = Mathf.Clamp01(distance / maxSwipeDistance);
        }

        OnSwipeUpdated?.Invoke();
    }

    private void OnTouchStarted(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        _startScreenPos = _touchPosition.ReadValue<Vector2>();
        StartWorldPos = _cam.ScreenToWorldPoint(_startScreenPos);
        _isSwiping = true;
        SwipeDirection = Vector2.zero;
        SwipePower = 0f;

        OnSwipeStarted?.Invoke();
    }

    private void OnTouchReleased(InputAction.CallbackContext ctx)
    {
        if (!_isSwiping) return;
        _isSwiping = false;

        if (SwipePower > 0f)
        {
            OnSwipeReleased?.Invoke(SwipeDirection, SwipePower);
        }
    }
}
