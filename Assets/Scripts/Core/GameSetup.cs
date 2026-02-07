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

        // DamagePopup singleton (editable in inspector if added manually, or auto-created here)
        if (DamagePopup.Instance == null)
        {
            var go = new GameObject("DamagePopupConfig");
            go.AddComponent<DamagePopup>();
        }
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
        wall.layer = LayerMask.NameToLayer("Wall");

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        col.sharedMaterial = new PhysicsMaterial2D("WallBounce") { bounciness = 1f, friction = 0f };
    }
}
