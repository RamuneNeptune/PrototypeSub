using SubLibrary.CyclopsReferencers;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class VolumetricLightReferenceAssigner : MonoBehaviour, ICyclopsReferencer
{
    [SerializeField] private VFXVolumetricLight volumetricLight;

    private void OnValidate()
    {
        if (!volumetricLight) TryGetComponent(out volumetricLight);
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        Plugin.Logger.LogInfo($"Cyclops reference finished");

        VFXVolumetricLight light = cyclops.transform.Find("Floodlights/VolumetricLight_Front").GetComponent<VFXVolumetricLight>();

        volumetricLight.coneMat = light.coneMat;
        volumetricLight.sphereMat = light.sphereMat;
        volumetricLight.block = light.block;
    }
}
