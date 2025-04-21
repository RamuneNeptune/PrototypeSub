using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class OnModifyPowerEvent : MonoBehaviour
{
    public event Action<float> onModifyPower;

    public void ModifiedPower(float power)
    {
        onModifyPower?.Invoke(power);
    }
}