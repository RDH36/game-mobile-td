using UnityEngine;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    private SpriteRenderer _sr;

    void Awake()
    {
        Instance = this;

        GameObject flashGO = new GameObject("FlashOverlay");
        flashGO.transform.SetParent(transform);
        flashGO.transform.localPosition = Vector3.zero;

        _sr = flashGO.AddComponent<SpriteRenderer>();
        _sr.sortingOrder = 999;

        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        _sr.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);

        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;
        flashGO.transform.localScale = new Vector3(camWidth + 1f, camHeight + 1f, 1f);

        _sr.color = new Color(1, 0, 0, 0);
    }

    public void Flash(Color color, float duration = 0.25f)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(color, duration));
    }

    public void FlashRed()
    {
        Flash(Color.red, 0.25f);
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        color.a = 0.4f;
        _sr.color = color;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0.4f, 0f, timer / duration);
            _sr.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        _sr.color = new Color(color.r, color.g, color.b, 0f);
    }
}
