using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class LightDistortionApplier : MonoBehaviour
{
    private Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CloakEffectHandler.Instance == null) return;

        if (!material) material = new Material(CloakEffectHandler.Instance.shader);

        Transform sphere = CloakEffectHandler.Instance.ovoid;
        material.SetVector("_OvoidCenter", sphere.position);
        material.SetVector("_OvoidRadii", sphere.localScale);
        material.SetFloat("_Multiplier", CloakEffectHandler.Instance.falloffMultiplier);
        material.SetColor("_Color", CloakEffectHandler.Instance.color);
        material.SetColor("_DistortionColor", CloakEffectHandler.Instance.distortionColor);
        material.SetFloat("_EffectBoundaryMin", CloakEffectHandler.Instance.distortionBoundaryMin);
        material.SetFloat("_EffectBoundaryMax", CloakEffectHandler.Instance.distortionBoundaryMax);
        material.SetFloat("_DistortionAmplitude", CloakEffectHandler.Instance.distortionAmplitude);
        material.SetFloat("_BoundaryOffset", CloakEffectHandler.Instance.distortionBoundaryOffset);

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(sphere.rotation);
        material.SetMatrix("_InverseRotationMatrix", rotationMatrix);

        Graphics.Blit(source, destination, material);
    }
}
