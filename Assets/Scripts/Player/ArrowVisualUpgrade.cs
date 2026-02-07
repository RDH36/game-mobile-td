using UnityEngine;

/// <summary>
/// Applies AllIn1SpriteShader visual effects to arrows based on upgrade bonuses.
/// Damage bonus → orange-red color tint + glow, Durability bonus → cyan outline.
/// </summary>
public static class ArrowVisualUpgrade
{
    private static Shader _allIn1Shader;

    public static void Apply(GameObject arrowGO, int bonusDamage, int bonusDurability)
    {
        if (bonusDamage <= 0 && bonusDurability <= 0) return;

        var sr = arrowGO.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Cache shader lookup
        if (_allIn1Shader == null)
            _allIn1Shader = Shader.Find("AllIn1SpriteShader/AllIn1SpriteShader");

        if (_allIn1Shader == null)
        {
            Debug.LogWarning("ArrowVisualUpgrade: AllIn1SpriteShader not found");
            return;
        }

        // Create material instance with AllIn1 shader
        Material mat = new Material(_allIn1Shader);
        mat.SetTexture("_MainTex", sr.sprite.texture);
        mat.SetColor("_Color", sr.color);

        // Damage bonus → HitEffect tint (orange-red) + Glow
        if (bonusDamage > 0)
        {
            // HitEffect: tints the entire sprite — very visible
            mat.EnableKeyword("HITEFFECT_ON");
            mat.SetColor("_HitEffectColor", new Color(1f, 0.3f, 0.05f, 1f));
            mat.SetFloat("_HitEffectGlow", 3f + bonusDamage * 2f);
            mat.SetFloat("_HitEffectBlend", Mathf.Clamp01(0.3f + bonusDamage * 0.15f));

            // Glow: adds outer glow around sprite
            mat.EnableKeyword("GLOW_ON");
            mat.SetColor("_GlowColor", new Color(1f, 0.4f, 0.1f, 1f));
            mat.SetFloat("_Glow", 10f + bonusDamage * 5f);
            mat.SetFloat("_GlowGlobal", 2f + bonusDamage * 1f);
        }

        // Durability bonus → Outline (cyan, thick)
        if (bonusDurability > 0)
        {
            mat.EnableKeyword("ALPHAOUTLINE_ON");
            mat.SetColor("_OutlineColor", new Color(0.1f, 0.9f, 1f, 1f));
            mat.SetFloat("_OutlineAlpha", 1f);
            mat.SetFloat("_OutlineWidth", 0.03f + bonusDurability * 0.015f);
            mat.SetFloat("_OutlineGlow", 4f + bonusDurability * 2f);
        }

        sr.material = mat;
    }
}
