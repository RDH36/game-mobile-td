using UnityEngine;
using TMPro;
using System.Collections;
using MoreMountains.Feedbacks;

public class DamagePopup : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private float fontSize = 3.5f;
    [SerializeField] private Color textColor = new Color(1f, 0.2f, 0.15f);
    [SerializeField] private float outlineWidth = 0.15f;
    [SerializeField] private Color outlineColor = new Color(0.3f, 0f, 0f);

    [Header("Animation")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float driftUpDistance = 1.0f;
    [SerializeField] private float spawnOffsetY = 0.4f;
    [SerializeField] private float randomOffsetX = 0.2f;
    [SerializeField] private float springBump = 0.3f;

    public static DamagePopup Instance { get; private set; }
    private static TMP_FontAsset _cachedFont;

    void Awake()
    {
        Instance = this;
    }

    public static void Create(Vector3 position, int damage)
    {
        if (_cachedFont == null)
            _cachedFont = Resources.Load<TMP_FontAsset>("UI/Showpop SDF");
        // Use instance settings if available, otherwise defaults
        float fSize = Instance != null ? Instance.fontSize : 3.5f;
        Color fColor = Instance != null ? Instance.textColor : new Color(1f, 0.2f, 0.15f);
        float fOutlineW = Instance != null ? Instance.outlineWidth : 0.15f;
        Color fOutlineC = Instance != null ? Instance.outlineColor : new Color(0.3f, 0f, 0f);
        float fDuration = Instance != null ? Instance.duration : 0.8f;
        float fDrift = Instance != null ? Instance.driftUpDistance : 1.0f;
        float fOffsetY = Instance != null ? Instance.spawnOffsetY : 0.4f;
        float fRandX = Instance != null ? Instance.randomOffsetX : 0.2f;
        float fBump = Instance != null ? Instance.springBump : 0.3f;

        GameObject go = new GameObject("DamagePopup");
        go.transform.position = position + Vector3.up * fOffsetY;
        go.transform.localScale = Vector3.zero;

        var tmp = go.AddComponent<TextMeshPro>();
        if (_cachedFont != null) tmp.font = _cachedFont;
        tmp.text = $"-{damage}";
        tmp.fontSize = fSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = fColor;
        tmp.outlineWidth = fOutlineW;
        tmp.outlineColor = fOutlineC;
        tmp.sortingOrder = 100;

        // Feel: spring scale for bouncy pop-in
        var spring = go.AddComponent<MMSpringScale>();
        spring.MoveToInstant(Vector3.zero);
        spring.MoveTo(Vector3.one);
        spring.Bump(new Vector3(fBump, fBump, 0f));

        var popup = go.AddComponent<DamagePopupAnim>();
        popup.Init(tmp, fDuration, fDrift, fRandX);
    }
}

public class DamagePopupAnim : MonoBehaviour
{
    private TextMeshPro _tmp;
    private float _duration;
    private float _drift;
    private float _randX;

    public void Init(TextMeshPro tmp, float duration, float drift, float randX)
    {
        _tmp = tmp;
        _duration = duration;
        _drift = drift;
        _randX = randX;
        StartCoroutine(AnimateAndDestroy());
    }

    IEnumerator AnimateAndDestroy()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float xOffset = Random.Range(-_randX, _randX);

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;

            float yMove = t * _drift;
            transform.position = startPos + new Vector3(xOffset * t, yMove, 0f);

            if (t > 0.5f)
            {
                float fadeT = (t - 0.5f) / 0.5f;
                _tmp.alpha = 1f - fadeT;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
