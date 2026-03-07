using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using UnityEngine;

namespace IceDragon.Framework.MaterialModifiers;

public class IceDragonMaterialModifier : MaterialModifier
{
    public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
    {
        if (materialType == MaterialUtils.MaterialType.Transparent)
        {
            material.color = new Color(1.5f, 1.5f, 1.5f);
            material.SetFloat("_Shininess", 5.5f);
            material.SetFloat("_SpecInt", 70);
            material.SetFloat("_IBLreductionAtNight", 0.25f);
            material.SetFloat("_Fresnel", 0.3f);
        }

        if (material.name.Contains("Pupil"))
        {
            material.SetFloat("_Shininess", 6f);
            material.SetFloat("_SpecInt", 10);
            material.SetFloat("_Fresnel", 0);
            material.SetFloat("_GlowStrength", 0.2f);
            material.SetFloat("_GlowStrengthNight", 0.2f);
        }
    }
}