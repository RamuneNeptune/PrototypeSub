using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class LightDistortionApplier : MonoBehaviour
{
    private static Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CloakEffectHandler.Instance == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (!material) material = new Material(CloakEffectHandler.Instance.shader);

        Transform sphere = CloakEffectHandler.Instance.ovoid;
        material.SetColor("_Color", CloakEffectHandler.Instance.color);
        material.SetColor("_DistortionColor", CloakEffectHandler.Instance.distortionColor);
        material.SetColor("_InteriorColor", CloakEffectHandler.Instance.interiorColor);
        material.SetColor("_VignetteColor", CloakEffectHandler.Instance.vignetteColor);
        material.SetVector("_OvoidCenter", sphere.position);
        material.SetVector("_OvoidRadii", sphere.localScale);
        material.SetFloat("_Multiplier", CloakEffectHandler.Instance.falloffMultiplier);
        material.SetFloat("_EffectBoundaryMin", CloakEffectHandler.Instance.distortionBoundaryMin);
        material.SetFloat("_EffectBoundaryMax", CloakEffectHandler.Instance.distortionBoundaryMax);
        material.SetFloat("_DistortionAmplitude", CloakEffectHandler.Instance.distortionAmplitude);
        material.SetFloat("_BoundaryOffset", CloakEffectHandler.Instance.distortionBoundaryOffset);
        material.SetFloat("_VignetteIntensity", CloakEffectHandler.Instance.vignetteIntensity);
        material.SetFloat("_VignetteSmoothness", CloakEffectHandler.Instance.vignetteSmoothness);
        material.SetFloat("_VignetteOffset", CloakEffectHandler.Instance.vignetteOffset);
        material.SetFloat("_VignetteFadeInDist", CloakEffectHandler.Instance.vignetteFadeInDist);

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(sphere.rotation);
        material.SetMatrix("_InverseRotationMatrix", rotationMatrix);

        Graphics.Blit(source, destination, material);
    }
}
