using SubLibrary.Handlers;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class VolumetricLightReferenceAssigner : MonoBehaviour
{
    [SerializeField] private VFXVolumetricLight volumetricLight;
    [SerializeField] private float fresnelFade;
    [SerializeField] private float fresnelPower;

    private void OnValidate()
    {
        if (!volumetricLight) TryGetComponent(out volumetricLight);
    }

    private void Awake()
    {
        if (CyclopsReferenceHandler.CyclopsReference == null) return;

        VFXVolumetricLight light = CyclopsReferenceHandler.CyclopsReference.transform.Find("Floodlights/VolumetricLight_Front").GetComponent<VFXVolumetricLight>();

        volumetricLight.coneMat = light.coneMat;
        volumetricLight.sphereMat = light.sphereMat;
        volumetricLight.block = light.block;
    }

    private void Start()
    {
        var rend = GetComponentInChildren<Renderer>();
        rend.material.SetFloat(ShaderPropertyID._FresnelFade, fresnelFade);
        rend.material.SetFloat("_FresnelPow", fresnelPower);
    }
}