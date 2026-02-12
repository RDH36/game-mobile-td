using UnityEngine;

public enum BossPattern
{
    Obstacle,   // Blob King - hides behind blocks
    Speed,      // Speedy - fast horizontal movement
    Shield,     // Guardian - invulnerable side
    Split,      // Splitter - divides at 50% HP
    Overlord    // All mechanics combined
}

[CreateAssetMenu(fileName = "BossData", menuName = "Game/BossData")]
public class BossData : ScriptableObject
{
    public string bossName;
    public int bossWave;          // Which wave this boss appears (10, 20, 30, 40, 50)
    public int maxHP = 15;
    public int damage = 2;
    public int coinDropMin = 20;
    public int coinDropMax = 40;
    public Color color = Color.white;
    public Sprite sprite;
    public RuntimeAnimatorController animController;
    public BossPattern pattern;
    public string skinRewardId;   // Skin unlocked on defeat (e.g., "Bronze", "Silver")
}
