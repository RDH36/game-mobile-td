using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private int _currentHP;
    private int _maxHP;

    public int CurrentHP => _currentHP;
    public bool IsDead => _currentHP <= 0;

    public event System.Action<int, int> OnHPChanged; // current, max
    public event System.Action<Enemy> OnDied;

    public void Init(int maxHP)
    {
        _maxHP = maxHP;
        _currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        _currentHP = Mathf.Max(0, _currentHP - damage);
        OnHPChanged?.Invoke(_currentHP, _maxHP);

        if (IsDead)
        {
            OnDied?.Invoke(GetComponent<Enemy>());
        }
    }
}
