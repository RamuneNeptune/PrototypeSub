using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Overclock;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class OverclockWaterManager : MonoBehaviour
{
    [SerializeField] private WaterMoveFXManager waterMoveManager;
    [SerializeField] private ProtoOverclockModule overclockModule;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private Gradient normalPilotingColors;
    [SerializeField] private Gradient overclockColors;
    
    private void Update()
    {
        var gradient = overclockModule.GetUpgradeEnabled() ? overclockColors : normalPilotingColors;

        var color = gradient.Evaluate(Mathf.Clamp01(motorHandler.GetNormalizedSpeed()));
        waterMoveManager.RegisterColor(this, 10, color, color);
    }
}