using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        // Create UI Canvas overlay (renders on top of everything)
        GameObject canvasGO = new GameObject("FlashCanvas");
        canvasGO.transform.SetParent(transform);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        // Create vignette flash image (transparent center, colored edges)
        GameObject imgGO = new GameObject("FlashImage");
        imgGO.transform.SetParent(canvasGO.transform, false);

        Image img = imgGO.AddComponent<Image>();
        img.sprite = CreateVignetteSprite(256);
        img.type = Image.Type.Simple;
        img.color = Color.white;
        img.raycastTarget = false;

        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Feel: MMFlash requires Image + CanvasGroup
        imgGO.AddComponent<CanvasGroup>();
        var flash = imgGO.AddComponent<MMFlash>();
        flash.FlashID = 0;
        flash.Interruptable = true;
    }

    Sprite CreateVignetteSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Normalized coords 0..1
                float u = (float)x / (size - 1);
                float v = (float)y / (size - 1);

                // Distance from nearest edge (0 = at edge, 0.5 = center)
                float distFromEdge = Mathf.Min(Mathf.Min(u, 1f - u), Mathf.Min(v, 1f - v));

                // Border: opaque at edge, transparent inside
                float borderWidth = 0.12f;
                float alpha = 1f - Mathf.Clamp01(distFromEdge / borderWidth);
                alpha = alpha * alpha; // quadratic for smooth falloff
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    public void Flash(Color color, float duration = 0.4f, float alpha = 0.7f)
    {
        MMFlashEvent.Trigger(color, duration, alpha, 0,
            new MMChannelData(MMChannelModes.Int, 0, null),
            TimescaleModes.Unscaled);
    }

    public void FlashRed()
    {
        Flash(Color.red, 0.3f, 0.35f);
    }
}
