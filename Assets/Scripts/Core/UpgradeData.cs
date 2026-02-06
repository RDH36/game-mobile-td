using UnityEngine;

public enum UpgradeType
{
    ArrowDurability,
    ArrowCount,
    ArrowDamage
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ArrowStrike/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public UpgradeType upgradeType;
    public int value = 1;
    public int cost = 10;
}
