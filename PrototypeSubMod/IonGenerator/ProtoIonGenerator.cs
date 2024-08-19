using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class ProtoIonGenerator : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private float energyPerSecond = 0.3f;
    [SerializeField] private float activeNoiseValue;

    private bool upgradeActive;



    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }

    public bool GetUpgradeActive() => upgradeActive;

    public float GetNoiseValue() => activeNoiseValue;
}
