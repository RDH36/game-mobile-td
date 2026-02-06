using UnityEngine;

/// <summary>
/// Applies bounce physics material to enemy collider so arrows bounce off.
/// </summary>
public class EnemyBounce : MonoBehaviour
{
    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.sharedMaterial = new PhysicsMaterial2D("EnemyBounce")
            {
                bounciness = 1f,
                friction = 0f
            };
        }
    }
}
