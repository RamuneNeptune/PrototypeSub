using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class ProtoIonGenerator : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private float energyPerSecond = 0.3f;
    [SerializeField] private float activeNoiseValue;

    private bool upgradeActive;
    private float[] originalSpeedValues;

    private void Start()
    {
        originalSpeedValues = motorMode.motorModeSpeeds;
    }

    private void Update()
    {
        if (!upgradeActive) return;

        motorMode.motorModeSpeeds = new float[originalSpeedValues.Length];
    }

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }

    public bool GetUpgradeActive() => upgradeActive;

    public float GetNoiseValue() => activeNoiseValue;
}
