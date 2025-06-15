using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

public class ProtoPowerRelayManager : MonoBehaviour
{
    [SerializeField] private ProtoDepthOptimizers depthOptimizers;

    public void ModifyPowerDrawn(ref float amount) => depthOptimizers.ModifyPowerDrawn(ref amount);
}