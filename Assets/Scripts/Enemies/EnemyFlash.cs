using UnityEngine;
using System.Collections;

public class EnemyFlash : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Color _originalColor;
    private Coroutine _flashRoutine;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    public void Flash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        _originalColor = _sr.color;
        _sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _sr.color = _originalColor;
        _flashRoutine = null;
    }
}
