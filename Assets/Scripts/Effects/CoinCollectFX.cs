using UnityEngine;
using TMPro;
using System.Collections;
using MoreMountains.Feedbacks;

public class CoinCollectFX : MonoBehaviour
{
    private static TMP_FontAsset _cachedFont;
    private static Sprite _cachedCoinSprite;

    public static void Create(Vector3 position, int gemAmount)
    {
        if (_cachedFont == null)
            _cachedFont = Resources.Load<TMP_FontAsset>("UI/Showpop SDF");
        if (_cachedCoinSprite == null)
        {
            var sprites = Resources.LoadAll<Sprite>("UI/coin_icon");
            if (sprites.Length > 0) _cachedCoinSprite = sprites[0];
        }

        // SFX once at spawn
        SFXManager.Instance?.PlayCoinCollect();

        // 1) "+N" text popup with Feel spring
        SpawnTextPopup(position, gemAmount);

        // 2) Flying coins: scatter then fly to HUD
        int coinCount = Mathf.Clamp(gemAmount, 1, 6);
        for (int i = 0; i < coinCount; i++)
            SpawnFlyingCoin(position, i * 0.06f);
    }

    static void SpawnTextPopup(Vector3 position, int gemAmount)
    {
        GameObject go = new GameObject("CoinPopupText");
        go.transform.position = position + Vector3.up * 0.5f;
        go.transform.localScale = Vector3.zero;

        var tmp = go.AddComponent<TextMeshPro>();
        if (_cachedFont != null) tmp.font = _cachedFont;
        tmp.text = $"+{gemAmount}";
        tmp.fontSize = 4f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.9f, 0.2f);
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = new Color(0.4f, 0.25f, 0f);
        tmp.sortingOrder = 105;

        // Feel: pop-in
        var scaleSpring = go.AddComponent<MMSpringScale>();
        scaleSpring.MoveToInstant(Vector3.zero);
        scaleSpring.MoveTo(Vector3.one);
        scaleSpring.Bump(new Vector3(0.3f, 0.3f, 0f));

        // Feel: drift up
        var posSpring = go.AddComponent<MMSpringPosition>();
        posSpring.MoveToInstant(go.transform.position);
        posSpring.MoveTo(go.transform.position + Vector3.up * 1.2f);

        // Fade + destroy (minimal coroutine just for alpha)
        go.AddComponent<CoinPopupFade>().Init(tmp, 0.9f);
    }

    static void SpawnFlyingCoin(Vector3 position, float delay)
    {
        if (_cachedCoinSprite == null) return;

        GameObject go = new GameObject("FlyingCoin");
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.5f;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = _cachedCoinSprite;
        sr.sortingOrder = 110;

        // Feel springs (added upfront, used in phases)
        var posSpring = go.AddComponent<MMSpringPosition>();
        var scaleSpring = go.AddComponent<MMSpringScale>();

        go.AddComponent<FlyingCoinController>().Init(posSpring, scaleSpring, delay);
    }
}

/// <summary>
/// Minimal: just fades alpha and destroys. Position handled by Feel MMSpringPosition.
/// </summary>
public class CoinPopupFade : MonoBehaviour
{
    private TextMeshPro _tmp;
    private float _duration;

    public void Init(TextMeshPro tmp, float duration)
    {
        _tmp = tmp;
        _duration = duration;
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;
            if (t > 0.5f)
                _tmp.alpha = 1f - (t - 0.5f) / 0.5f;
            yield return null;
        }
        Destroy(gameObject);
    }
}

/// <summary>
/// Flying coin: scatter via Feel spring → pause → fly to HUD via Feel spring → bump + destroy
/// </summary>
public class FlyingCoinController : MonoBehaviour
{
    private MMSpringPosition _posSpring;
    private MMSpringScale _scaleSpring;

    public void Init(MMSpringPosition posSpring, MMSpringScale scaleSpring, float delay)
    {
        _posSpring = posSpring;
        _scaleSpring = scaleSpring;
        StartCoroutine(RunPhases(delay));
    }

    IEnumerator RunPhases(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Vector3 origin = transform.position;

        // Phase 1: Scatter outward with Feel spring
        Vector2 scatterDir = Random.insideUnitCircle.normalized;
        float scatterDist = Random.Range(0.5f, 0.9f);
        Vector3 scatterTarget = origin + (Vector3)(scatterDir * scatterDist);

        _posSpring.MoveToInstant(origin);
        _posSpring.MoveTo(scatterTarget);
        _scaleSpring.Bump(new Vector3(0.2f, 0.2f, 0f));

        // Wait for scatter to settle
        yield return new WaitForSeconds(0.4f);

        // Phase 2: Fly to HUD gem counter with Feel spring
        Vector3 flyTarget = GetGemCounterWorldPos();
        _posSpring.MoveTo(flyTarget);

        // Shrink while flying
        _scaleSpring.MoveTo(Vector3.one * 0.15f);

        // Wait for fly to reach target
        yield return new WaitForSeconds(0.5f);

        // On arrival: bump HUD only (SFX already played at spawn)
        HUDManager.BumpGemCounter();

        Destroy(gameObject);
    }

    Vector3 GetGemCounterWorldPos()
    {
        if (HUDManager.GemCounterWorldPos != Vector3.zero)
            return HUDManager.GemCounterWorldPos;

        // Fallback: top-left area of screen
        Camera cam = Camera.main;
        if (cam != null)
        {
            float camDist = Mathf.Abs(cam.transform.position.z);
            Vector3 wp = cam.ScreenToWorldPoint(new Vector3(100f, Screen.height - 80f, camDist));
            return new Vector3(wp.x, wp.y, 0f);
        }
        return transform.position + Vector3.up * 3f;
    }
}
