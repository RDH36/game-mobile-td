using UnityEditor;
using UnityEngine;

public static class CreateBounceMaterial
{
    [MenuItem("Tools/Create Bounce Material")]
    public static void Execute()
    {
        // Arrow bounce material - perfect bounce, no friction
        PhysicsMaterial2D arrowBounce = new PhysicsMaterial2D("ArrowBounce");
        arrowBounce.bounciness = 1f;
        arrowBounce.friction = 0f;
        AssetDatabase.CreateAsset(arrowBounce, "Assets/Physics/ArrowBounce.physicsMaterial2D");

        // Wall material - perfect bounce
        PhysicsMaterial2D wallBounce = new PhysicsMaterial2D("WallBounce");
        wallBounce.bounciness = 1f;
        wallBounce.friction = 0f;
        AssetDatabase.CreateAsset(wallBounce, "Assets/Physics/WallBounce.physicsMaterial2D");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Bounce materials created at Assets/Physics/");
    }
}
