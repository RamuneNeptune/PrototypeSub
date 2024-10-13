using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using UnityEngine;

namespace PrototypeSubMod.Utility;

// This class is pretty much a copy of the PrecursorMaterialModifier in RotA, which coincidentally is more feature-rich. Make sure to check it out!
internal class ProtoMaterialModifier : MaterialModifier
{
    private readonly float _specInt;
    private readonly float _fresnelStrength;

    private static readonly int _specIntID = Shader.PropertyToID("_SpecInt");

    private static readonly int _fresnelID = Shader.PropertyToID("_Fresnel");

    private static Color precursorSpecularGreen = new Color(0.25f, 0.54f, 0.41f);

    public ProtoMaterialModifier(float specInt, float fresnelStrength = 0.4f)
    {
        _specInt = specInt;
        _fresnelStrength = fresnelStrength;
    }

    public override bool BlockShaderConversion(Material material, Renderer renderer, MaterialUtils.MaterialType materialType)
    {
        return renderer is ParticleSystemRenderer;
    }

    public override void EditMaterial(Material material, Renderer renderer, int materialIndex, MaterialUtils.MaterialType materialType)
    {
        if (renderer.TryGetComponent<DontApplyProtoMaterial>(out _)) return;

        string matName = material.name.ToLower();
        if (matName.Contains("transparent"))
        {
            var materials = renderer.materials;
            materials[materialIndex] = MaterialUtils.PrecursorGlassMaterial;
            renderer.materials = materials;
        }

        material.SetColor(ShaderPropertyID._SpecColor, precursorSpecularGreen);
        material.SetFloat(_specIntID, _specInt);
        material.SetFloat(_fresnelID, _fresnelStrength);
    }
}
