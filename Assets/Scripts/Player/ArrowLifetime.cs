using UnityEngine;

public class ArrowLifetime : MonoBehaviour
{
    [SerializeField] private float maxLifetime = 5f;

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }
}
