using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class LightDistortionApplier : MonoBehaviour
{
    private Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CloakEffectHandler.Instance == null) return;

        if (!material) material = new Material(CloakEffectHandler.Instance.GetShader());

        Transform sphere = CloakEffectHandler.Instance.GetSphere();
        material.SetVector("SphereCenter", sphere.position);
        material.SetFloat("SphereRadius", sphere.localScale.x);
        material.SetFloat("_Multiplier", CloakEffectHandler.Instance.GetFalloffMultiplier());
        material.SetColor("_Color", CloakEffectHandler.Instance.GetEffectColor());

        Graphics.Blit(source, destination, material);
    }
}
