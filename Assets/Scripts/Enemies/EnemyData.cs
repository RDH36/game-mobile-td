using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Monster Cannon/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHP = 1;
    public int damage = 1;
    public Color color = Color.white;
    public Sprite sprite;
    public RuntimeAnimatorController animController;
    public int gemDropMin = 1;
    public int gemDropMax = 2;
}
