using UnityEngine;

/// <summary>
/// Sprite-based health bar that sits above an enemy.
/// Uses enemy hp bar bg/fg sprites loaded from Resources/UI/.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] private float yOffset = 0.15f;
    [SerializeField] private float barScale = 0.4f;

    private SpriteRenderer _fg;
    private float _fgFullWidth;
    private GameObject _root;

    public void Setup(float spriteHeight)
    {
        // LoadAll handles both Single and Multiple sprite modes
        var bgSprites = Resources.LoadAll<Sprite>("UI/enemy_hp_bar_bg");
        var fgSprites = Resources.LoadAll<Sprite>("UI/enemy_hp_bar_fg");
        Sprite bgSprite = bgSprites.Length > 0 ? bgSprites[0] : null;
        Sprite fgSprite = fgSprites.Length > 0 ? fgSprites[0] : null;

        if (bgSprite == null || fgSprite == null)
        {
            Debug.LogWarning("EnemyHealthBar: sprites not found in Resources/UI/");
            return;
        }

        float actualOffset = spriteHeight * 0.5f + yOffset;

        _root = new GameObject("HealthBar");
        _root.transform.SetParent(transform, false);
        _root.transform.localPosition = new Vector3(0f, actualOffset, 0f);
        _root.transform.localScale = Vector3.one * barScale;

        // Background (frame)
        var bgGO = new GameObject("BG");
        bgGO.transform.SetParent(_root.transform, false);
        var bg = bgGO.AddComponent<SpriteRenderer>();
        bg.sprite = bgSprite;
        bg.sortingOrder = 9;

        // Foreground (fill)
        var fgGO = new GameObject("FG");
        fgGO.transform.SetParent(_root.transform, false);
        _fg = fgGO.AddComponent<SpriteRenderer>();
        _fg.sprite = fgSprite;
        _fg.sortingOrder = 10;

        _fgFullWidth = _fg.sprite.bounds.size.x;
    }

    public void UpdateBar(int current, int max)
    {
        if (_fg == null) return;

        float ratio = Mathf.Clamp01((float)current / max);

        // Scale x for fill, keep y/z unchanged
        _fg.transform.localScale = new Vector3(ratio, 1f, 1f);

        // Offset x to keep left edge anchored
        float offset = -_fgFullWidth * (1f - ratio) * 0.5f;
        _fg.transform.localPosition = new Vector3(offset, 0f, 0f);

        // Hide at zero
        if (current <= 0 && _root != null)
            _root.SetActive(false);
    }
}
