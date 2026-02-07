using UnityEngine;
using TMPro;
using System.Collections;
using MoreMountains.Feedbacks;

/// <summary>
/// Coin sprite pop effect when gems are collected from killed enemies.
/// Spawns coin icon + gem count text with Feel spring animation.
/// </summary>
public class CoinCollectFX : MonoBehaviour
{
    private static TMP_FontAsset _cachedFont;

    public static void Create(Vector3 position, int gemAmount)
    {
        // Load coin sprite
        var coinSprites = Resources.LoadAll<Sprite>("UI/coin_icon");
        Sprite coinSprite = coinSprites.Length > 0 ? coinSprites[0] : null;

        // Load font
        if (_cachedFont == null)
            _cachedFont = Resources.Load<TMP_FontAsset>("UI/Showpop SDF");

        GameObject root = new GameObject("CoinCollectFX");
        root.transform.position = position + Vector3.up * 0.3f;
        root.transform.localScale = Vector3.zero;

        // Coin sprite
        if (coinSprite != null)
        {
            GameObject coinGO = new GameObject("CoinIcon");
            coinGO.transform.SetParent(root.transform, false);
            coinGO.transform.localPosition = new Vector3(-0.25f, 0f, 0f);
            coinGO.transform.localScale = Vector3.one * 0.4f;
            var sr = coinGO.AddComponent<SpriteRenderer>();
            sr.sprite = coinSprite;
            sr.sortingOrder = 105;
        }

        // "+N" text next to coin
        GameObject textGO = new GameObject("GemText");
        textGO.transform.SetParent(root.transform, false);
        textGO.transform.localPosition = new Vector3(0.2f, 0f, 0f);
        var tmp = textGO.AddComponent<TextMeshPro>();
        if (_cachedFont != null) tmp.font = _cachedFont;
        tmp.text = $"+{gemAmount}";
        tmp.fontSize = 3f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.color = new Color(1f, 0.9f, 0.2f); // gold
        tmp.outlineWidth = 0.15f;
        tmp.outlineColor = new Color(0.4f, 0.25f, 0f);
        tmp.sortingOrder = 105;

        // Feel: spring scale for pop-in
        var spring = root.AddComponent<MMSpringScale>();
        spring.MoveToInstant(Vector3.zero);
        spring.MoveTo(Vector3.one);
        spring.Bump(new Vector3(0.3f, 0.3f, 0f));

        // Animate drift up + fade
        root.AddComponent<CoinCollectAnim>().Init(tmp, 1.0f);
    }
}

public class CoinCollectAnim : MonoBehaviour
{
    private TextMeshPro _tmp;
    private float _duration;
    private SpriteRenderer _coinSR;

    public void Init(TextMeshPro tmp, float duration)
    {
        _tmp = tmp;
        _duration = duration;
        _coinSR = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(AnimateAndDestroy());
    }

    IEnumerator AnimateAndDestroy()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float xOffset = Random.Range(-0.15f, 0.15f);

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;

            // Drift up
            transform.position = startPos + new Vector3(xOffset * t, t * 0.8f, 0f);

            // Fade out last 40%
            if (t > 0.6f)
            {
                float fadeT = (t - 0.6f) / 0.4f;
                float alpha = 1f - fadeT;
                _tmp.alpha = alpha;
                if (_coinSR != null)
                {
                    Color c = _coinSR.color;
                    c.a = alpha;
                    _coinSR.color = c;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
