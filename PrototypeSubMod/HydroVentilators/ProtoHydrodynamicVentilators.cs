using PrototypeSubMod.Interfaces;
using PrototypeSubMod.MotorHandler;
using UnityEngine;

namespace PrototypeSubMod.HydroVentilators;

internal class ProtoHydrodynamicVentilators : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

    private bool upgradeActive;

    private void FixedUpdate()
    {
        if (!upgradeActive) return;

        float depth = crushDamage.GetDepth();

        if (depth < activationDepth)
        {
            motorHandler.SetSpeedMultiplier(1f);
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        motorHandler.SetSpeedMultiplier(multiplier);
    }

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }

    public bool GetUpgradeActive() => upgradeActive;

    public string GetUpgradeName()
    {
        return "Hydro Ventilators";
    }
}
