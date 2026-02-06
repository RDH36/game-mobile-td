using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    private EnemyData _data;
    private EnemyHealth _health;
    private SpriteRenderer _sr;

    public EnemyData Data => _data;
    public EnemyHealth Health => _health;

    public void Init(EnemyData data)
    {
        _data = data;
        _health = GetComponent<EnemyHealth>();
        _sr = GetComponent<SpriteRenderer>();

        _health.Init(data.maxHP);
        _sr.color = data.color;

        gameObject.name = $"Enemy_{data.enemyName}";
    }

    public int GetGemDrop()
    {
        return Random.Range(_data.gemDropMin, _data.gemDropMax + 1);
    }
}
