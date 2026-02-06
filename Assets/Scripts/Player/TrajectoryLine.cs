using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] private float bouncePreviewLength = 4f;
    [SerializeField] private LayerMask bounceLayerMask;

    private LineRenderer _line;
    private SwipeDetector _swipe;
    private BowController _bow;

    void Awake()
    {
        _line = GetComponent<LineRenderer>();
        _swipe = GetComponentInParent<SwipeDetector>();
        _bow = GetComponentInParent<BowController>();
        SetupLineRenderer();
        _line.enabled = false;
    }

    void OnEnable()
    {
        if (_swipe != null)
        {
            _swipe.OnSwipeStarted += ShowLine;
            _swipe.OnSwipeUpdated += UpdateLine;
            _swipe.OnSwipeReleased += HideLine;
        }
    }

    void OnDisable()
    {
        if (_swipe != null)
        {
            _swipe.OnSwipeStarted -= ShowLine;
            _swipe.OnSwipeUpdated -= UpdateLine;
            _swipe.OnSwipeReleased -= HideLine;
        }
    }

    void SetupLineRenderer()
    {
        _line.startWidth = 0.05f;
        _line.endWidth = 0.05f;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.startColor = new Color(1f, 1f, 1f, 0.6f);
        _line.endColor = new Color(1f, 1f, 1f, 0.2f);
        _line.textureMode = LineTextureMode.Tile;
        _line.sortingOrder = 5;

        // Dotted line pattern
        _line.material.mainTexture = CreateDottedTexture();
        _line.textureScale = new Vector2(3f, 1f);
    }

    Texture2D CreateDottedTexture()
    {
        Texture2D tex = new Texture2D(8, 1);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Point;
        for (int i = 0; i < 8; i++)
        {
            tex.SetPixel(i, 0, i < 4 ? Color.white : Color.clear);
        }
        tex.Apply();
        return tex;
    }

    void ShowLine()
    {
        _line.enabled = true;
    }

    void UpdateLine()
    {
        if (!_bow.IsAiming)
        {
            _line.positionCount = 0;
            return;
        }

        Vector2 direction = _swipe.SwipeDirection;

        // Only allow shooting upward
        if (direction.y < 0.1f)
        {
            _line.positionCount = 0;
            return;
        }
        direction.Normalize();

        Vector2 bowPos = transform.position;
        Vector2 rayStart = bowPos + direction * 0.6f;

        // Raycast to find first bounce point
        RaycastHit2D hit = Physics2D.Raycast(rayStart, direction, 50f, bounceLayerMask);

        if (hit.collider != null)
        {
            // Bow → hit point → short bounce preview
            Vector2 reflected = Vector2.Reflect(direction, hit.normal);
            Vector2 bounceEnd = hit.point + reflected * bouncePreviewLength;

            _line.positionCount = 3;
            _line.SetPosition(0, bowPos);
            _line.SetPosition(1, (Vector3)hit.point);
            _line.SetPosition(2, (Vector3)bounceEnd);
        }
        else
        {
            // No wall hit — just show direction
            _line.positionCount = 2;
            _line.SetPosition(0, bowPos);
            _line.SetPosition(1, (Vector3)(rayStart + direction * 4f));
        }
    }

    void HideLine(Vector2 dir, float power)
    {
        _line.enabled = false;
        _line.positionCount = 0;
    }
}
