using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class CloakCutoutApplier : MonoBehaviour
{
    private static Material material;
    private DefenseCloakManager defenseCloakManager;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!defenseCloakManager || Plugin.GlobalSaveData.defenseCloakDisabled)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (!material) material = new Material(defenseCloakManager.shader);

        if (defenseCloakManager.isDirty)
        {
            RefreshVariables();
            defenseCloakManager.isDirty = false;
        }

        material.SetVector("_SphereCenter", defenseCloakManager.sphere.position);
        material.SetFloat("_SphereRadius", defenseCloakManager.sphere.lossyScale.x);

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(defenseCloakManager.hexPrism.rotation);
        material.SetMatrix("_HexRotationMatrix", rotationMatrix);

        material.SetVector("_HexCenter", defenseCloakManager.hexPrism.position);
        material.SetFloat("_HexRadius", defenseCloakManager.hexPrism.lossyScale.x);
        material.SetFloat("_HexHeight", defenseCloakManager.hexPrism.lossyScale.y);

        Graphics.Blit(source, destination, material);
    }

    private void RefreshVariables()
    {
        material.SetFloat("_EffectBoundaryMin", defenseCloakManager.effectBoundaryMin);
        material.SetFloat("_EffectBoundaryMax", defenseCloakManager.effectBoundaryMax);

        material.SetFloat("_BoundaryOffset", defenseCloakManager.boundaryOffset);
        material.SetFloat("_DistortionAmplitude", defenseCloakManager.distortionAmplitude);
        material.SetFloat("_VignetteIntensity", defenseCloakManager.vignetteIntensity);
        material.SetFloat("_VignetteSmoothness", defenseCloakManager.vignetteSmoothness);
        material.SetFloat("_VignetteOffset", defenseCloakManager.vignetteOffset);
        material.SetFloat("_VignetteFadeInDist", defenseCloakManager.vignetteFadeInDist);

        material.SetColor("_DistortionColor", defenseCloakManager.distortionColor);
        material.SetColor("_InteriorColor", defenseCloakManager.interiorColor);
        material.SetColor("_VignetteColor", defenseCloakManager.vignetteColor);
        material.SetColor("_OutsideInColor", defenseCloakManager.outsideInColor);

        material.SetFloat("_OscillationFrequency", defenseCloakManager.oscillationFrequency);
        material.SetFloat("_OscillationAmplitude", defenseCloakManager.oscillationAmplitude);
        material.SetFloat("_OscillationSpeed", defenseCloakManager.oscillationSpeed);
        material.SetFloat("_WaveCount", defenseCloakManager.waveCount);
        material.SetFloat("_AmplitudeFalloff", defenseCloakManager.amplitudeFalloff);
        material.SetFloat("_FrequencyIncrease", defenseCloakManager.frequencyIncrease);

        material.SetFloat("_EnabledAmount", defenseCloakManager.enabledAmount);
        material.SetFloat("_ExteriorCutoutRatio", defenseCloakManager.exteriorCutoutRatio);
    }

    public void SetCloakManager(DefenseCloakManager cloakManager)
    {
        defenseCloakManager = cloakManager;
    }
}
