using UnityEngine;

public class ArrowDurability : MonoBehaviour
{
    [SerializeField] private int maxHP = 4;
    private int _currentHP;

    public int CurrentHP => _currentHP;

    public event System.Action OnArrowDestroyed;

    void Awake()
    {
        _currentHP = maxHP;
    }

    public void TakeHit()
    {
        _currentHP--;
        if (_currentHP <= 0)
        {
            OnArrowDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }

    public void SetMaxHP(int hp)
    {
        maxHP = hp;
        _currentHP = hp;
    }
}
