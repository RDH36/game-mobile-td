using UnityEngine;

/// <summary>
/// Creates the 4 wall colliders around the screen edges at runtime.
/// Attach to an empty GameObject in the scene.
/// </summary>
public class GameSetup : MonoBehaviour
{
    void Awake()
    {
        CreateWalls();
    }

    void CreateWalls()
    {
        Camera cam = Camera.main;

        // Convert safe area from screen pixels to world coordinates
        Rect safe = Screen.safeArea;
        Vector2 safeMin = cam.ScreenToWorldPoint(new Vector2(safe.xMin, safe.yMin));
        Vector2 safeMax = cam.ScreenToWorldPoint(new Vector2(safe.xMax, safe.yMax));

        float safeWidth = safeMax.x - safeMin.x;
        float safeHeight = safeMax.y - safeMin.y;
        Vector2 safeCenter = (safeMin + safeMax) / 2f;

        float wallThickness = 0.3f;

        // Bottom wall at the bow position — bow sits on top of it
        float bottomWallY = -4f;
        CreateWall("Wall_Bottom", new Vector2(safeCenter.x, bottomWallY), new Vector2(safeWidth, wallThickness));
        // Top
        CreateWall("Wall_Top", new Vector2(safeCenter.x, safeMax.y), new Vector2(safeWidth, wallThickness));
        // Left — only above the bottom wall
        float arenaHeight = safeMax.y - bottomWallY;
        float arenaCenterY = (bottomWallY + safeMax.y) / 2f;
        CreateWall("Wall_Left", new Vector2(safeMin.x, arenaCenterY), new Vector2(wallThickness, arenaHeight));
        // Right — only above the bottom wall
        CreateWall("Wall_Right", new Vector2(safeMax.x, arenaCenterY), new Vector2(wallThickness, arenaHeight));
    }

    void CreateWall(string name, Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject(name);
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);
        wall.layer = LayerMask.NameToLayer("Wall");

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.sharedMaterial = new PhysicsMaterial2D("WallBounce") { bounciness = 1f, friction = 0f };

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = CreateWhiteSprite();
        sr.color = new Color(0.3f, 0.3f, 0.4f, 1f);
        sr.sortingOrder = -10;
    }

    private static Sprite _whiteSprite;
    Sprite CreateWhiteSprite()
    {
        if (_whiteSprite != null) return _whiteSprite;
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        _whiteSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        return _whiteSprite;
    }
}
