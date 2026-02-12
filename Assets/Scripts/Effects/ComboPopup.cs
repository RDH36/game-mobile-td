using UnityEngine;
using TMPro;
using System.Collections;
using MoreMountains.Feedbacks;

/// <summary>
/// Shows "COMBO xN!" text with Feel spring pop-in + gold flash + camera shake.
/// Triggered when 3+ kills with a single arrow.
/// </summary>
public class ComboPopup : MonoBehaviour
{
    private static TMP_FontAsset _cachedFont;

    public static void Create(int comboCount, int bonusGems)
    {
        // Load font
        if (_cachedFont == null)
            _cachedFont = Resources.Load<TMP_FontAsset>("UI/Showpop SDF");

        // Position at screen center in world space
        Camera cam = Camera.main;
        Vector3 center = cam != null ? cam.transform.position + Vector3.forward * 10f : Vector3.zero;
        center.z = 0f;

        GameObject go = new GameObject("ComboPopup");
        go.transform.position = center + Vector3.up * 1f;
        go.transform.localScale = Vector3.zero;

        // "COMBO x3!" text
        var tmp = go.AddComponent<TextMeshPro>();
        if (_cachedFont != null) tmp.font = _cachedFont;
        tmp.text = $"COMBO x{comboCount}!";
        tmp.fontSize = 6f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.85f, 0.1f); // gold
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = new Color(0.6f, 0.3f, 0f); // dark gold outline
        tmp.sortingOrder = 110;

        // "+N gems" sub-text below
        GameObject subGO = new GameObject("BonusText");
        subGO.transform.SetParent(go.transform, false);
        subGO.transform.localPosition = new Vector3(0f, -0.7f, 0f);
        var subTmp = subGO.AddComponent<TextMeshPro>();
        if (_cachedFont != null) subTmp.font = _cachedFont;
        subTmp.text = $"+{bonusGems} coins";
        subTmp.fontSize = 3.5f;
        subTmp.fontStyle = FontStyles.Bold;
        subTmp.alignment = TextAlignmentOptions.Center;
        subTmp.color = new Color(1f, 0.95f, 0.5f); // light gold
        subTmp.sortingOrder = 110;

        // Feel: spring scale for bouncy pop-in
        var spring = go.AddComponent<MMSpringScale>();
        spring.MoveToInstant(Vector3.zero);
        spring.MoveTo(Vector3.one * 1.2f);
        spring.Bump(new Vector3(0.5f, 0.5f, 0f));

        // Camera shake (stronger for combos)
        float shakeIntensity = 0.2f + comboCount * 0.1f;
        CameraShake.Instance?.Shake(shakeIntensity, 0.35f);

        // Gold screen flash
        if (ScreenFlash.Instance != null)
            ScreenFlash.Instance.Flash(new Color(1f, 0.85f, 0.1f), 0.3f, 0.2f);

        // Animate and destroy
        go.AddComponent<ComboPopupAnim>().Init(tmp, subTmp, 1.5f);
    }
}

public class ComboPopupAnim : MonoBehaviour
{
    private TextMeshPro _mainTmp;
    private TextMeshPro _subTmp;
    private float _duration;

    public void Init(TextMeshPro mainTmp, TextMeshPro subTmp, float duration)
    {
        _mainTmp = mainTmp;
        _subTmp = subTmp;
        _duration = duration;
        StartCoroutine(AnimateAndDestroy());
    }

    IEnumerator AnimateAndDestroy()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;

            // Slow drift up
            transform.position = startPos + Vector3.up * (t * 0.8f);

            // Fade out in last 40%
            if (t > 0.6f)
            {
                float fadeT = (t - 0.6f) / 0.4f;
                _mainTmp.alpha = 1f - fadeT;
                _subTmp.alpha = 1f - fadeT;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
