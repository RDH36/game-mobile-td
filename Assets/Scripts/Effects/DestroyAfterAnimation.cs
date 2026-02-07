using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Update()
    {
        var state = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        if (state.normalizedTime >= 1f)
            Destroy(gameObject);
    }
}
