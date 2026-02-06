using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Arrow Strike/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHP = 1;
    public int damage = 1;
    public Color color = Color.green;
    public int gemDropMin = 1;
    public int gemDropMax = 2;
}
