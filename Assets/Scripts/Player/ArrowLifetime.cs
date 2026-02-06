using UnityEngine;

public class ArrowLifetime : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 8f;

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }
}
