using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class OnModifyPowerEvent : MonoBehaviour
{
    public event Action<float> onModifyPower;

    public void ModiedPower(float power)
    {
        onModifyPower?.Invoke(power);
    }
}