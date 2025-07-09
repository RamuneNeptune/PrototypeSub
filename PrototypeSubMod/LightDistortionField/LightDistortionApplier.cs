using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class LightDistortionApplier : MonoBehaviour
{
    private static Material material;
    private CloakEffectHandler activeCloakHandler;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (activeCloakHandler == null || !activeCloakHandler.GetUpgradeInstalled())
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (!material) material = new Material(activeCloakHandler.shader);

        Transform sphere = activeCloakHandler.ovoid;

        if (activeCloakHandler.GetIsDirty())
        {
            RefreshVariables();
            activeCloakHandler.ClearDirty();
        }

        material.SetVector("_OvoidCenter", sphere.position);
        material.SetVector("_OvoidRadii", sphere.localScale);

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(sphere.rotation);
        material.SetMatrix("_InverseRotationMatrix", rotationMatrix);

        Graphics.Blit(source, destination, material);
    }

    private void RefreshVariables()
    {
        material.SetColor("_Color", activeCloakHandler.color);
        material.SetColor("_DistortionColor", activeCloakHandler.distortionColor);
        material.SetColor("_InteriorColor", activeCloakHandler.interiorColor);
        material.SetColor("_VignetteColor", activeCloakHandler.vignetteColor);
        material.SetFloat("_Multiplier", activeCloakHandler.falloffMultiplier);
        material.SetFloat("_EffectBoundaryMin", activeCloakHandler.distortionBoundaryMin);
        material.SetFloat("_EffectBoundaryMax", activeCloakHandler.distortionBoundaryMax);
        material.SetFloat("_DistortionAmplitude", activeCloakHandler.distortionAmplitude);
        material.SetFloat("_BoundaryOffset", activeCloakHandler.distortionBoundaryOffset);
        material.SetFloat("_VignetteIntensity", activeCloakHandler.vignetteIntensity);
        material.SetFloat("_VignetteSmoothness", activeCloakHandler.vignetteSmoothness);
        material.SetFloat("_VignetteOffset", activeCloakHandler.vignetteOffset);
        material.SetFloat("_VignetteFadeInDist", activeCloakHandler.vignetteFadeInDist);
        material.SetFloat("_OscillationFrequency", activeCloakHandler.oscillationFrequency);
        material.SetFloat("_OscillationAmplitude", activeCloakHandler.oscillationAmplitude);
        material.SetFloat("_OscillationSpeed", activeCloakHandler.oscillationSpeed);
        material.SetInt("_WaveCount", activeCloakHandler.waveCount);
        material.SetFloat("_FrequencyIncrease", activeCloakHandler.frequencyIncrease);
        material.SetFloat("_AmplitudeFalloff", activeCloakHandler.amplitudeFalloff);
        material.SetFloat("_FalloffMin", activeCloakHandler.falloffMin);
        material.SetFloat("_FalloffMax", activeCloakHandler.falloffMax);
    }

    public void RegisterCloakHandler(CloakEffectHandler handler)
    {
        activeCloakHandler = handler;
    }
}
