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
            var volum = atmosphereVolumes[i];
            volum.overrideBiome = $"{baseBiomeName}{i + 1}";
            volum.settings.overrideBiome = volum.overrideBiome;
        }
    }

    public void SetBiomesReversed()
    {
        for (int i = 0; i < atmosphereVolumes.Length; i++)
        {
            var volum = atmosphereVolumes[i];
            volum.overrideBiome = $"{baseBiomeName}{atmosphereVolumes.Length - i}";
            volum.settings.overrideBiome = volum.overrideBiome;
        }
    }
}
