using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Monster Cannon/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHP = 1;
    public int damage = 1;
    public Color color = Color.white;
    public Sprite sprite;
    public RuntimeAnimatorController animController;
    [FormerlySerializedAs("gemDropMin")] public int coinDropMin = 1;
    [FormerlySerializedAs("gemDropMax")] public int coinDropMax = 2;

    [Header("Visual Variants (random pick on spawn)")]
    public VisualVariant[] variants;

    [System.Serializable]
    public struct VisualVariant
    {
        public Sprite sprite;
        public RuntimeAnimatorController animController;
    }

    /// <summary>Returns a random visual variant (sprite + anim). Falls back to default if no variants.</summary>
    public (Sprite sprite, RuntimeAnimatorController anim) GetRandomVisual()
    {
        if (variants == null || variants.Length == 0)
            return (sprite, animController);

        int idx = Random.Range(0, variants.Length);
        var v = variants[idx];
        return (v.sprite != null ? v.sprite : sprite,
                v.animController != null ? v.animController : animController);
    }
}
