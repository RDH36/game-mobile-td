using UnityEngine;

public class BowHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 20;
    private int currentHP;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public event System.Action<int, int> OnHPChanged; // current, max
    public event System.Action OnBowDestroyed;

    void Awake()
    {
        currentHP = maxHP;
    }

    public event System.Action<int> OnDamageTaken; // damage amount

    public void TakeDamage(int damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage);
        SFXManager.Instance?.PlayCannonDamage();
        OnHPChanged?.Invoke(currentHP, maxHP);
        OnDamageTaken?.Invoke(damage);

        if (currentHP <= 0)
        {
            OnBowDestroyed?.Invoke();
            GameManager.Instance?.SetState(GameState.GameOver);
        }
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    public void ResetHP()
    {
        currentHP = maxHP;
        OnHPChanged?.Invoke(currentHP, maxHP);
    }
}
