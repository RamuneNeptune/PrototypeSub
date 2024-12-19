using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class DefenseTunnelBiomeManager : MonoBehaviour
{
    [SerializeField] private AtmosphereVolume[] atmosphereVolumes;
    [SerializeField] private string baseBiomeName;

    public void SetBiomesNormal()
    {
        for (int i = 0; i < atmosphereVolumes.Length; i++)
        {
            atmosphereVolumes[i].overrideBiome = $"{baseBiomeName}{i + 1}";
        }
    }

    public void SetBiomesReversed()
    {
        for (int i = 0; i < atmosphereVolumes.Length; i++)
        {
            atmosphereVolumes[atmosphereVolumes.Length - 1 - i].overrideBiome = $"{baseBiomeName}{i + 1}";
        }
    }
}
