using System;
using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

public class ProtoPowerRelayManager : MonoBehaviour
{
    private IPowerModifier[] powerModifiers;
    
    private void Start()
    {
        powerModifiers = GetComponentsInChildren<IPowerModifier>(true);
    }

    public void ModifyPowerDrawn(ref float amount)
    {
        foreach (var modifier in powerModifiers)
        {
            modifier.ModifyPowerDrawn(ref amount);
        }
    }
}

public interface IPowerModifier
{
    public void ModifyPowerDrawn(ref float amount);
}