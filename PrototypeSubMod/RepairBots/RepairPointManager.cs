using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class RepairPointManager : MonoBehaviour
{
    public event Action<CyclopsDamagePoint> onRepairPointCreated;
    public event Action<CyclopsDamagePoint> onRepairPointRepaired;

    public void OnDamagePointCreated(CyclopsDamagePoint point)
    {
        onRepairPointCreated?.Invoke(point);
    }

    public void OnDamagePointRepaired(CyclopsDamagePoint point)
    {
        onRepairPointRepaired?.Invoke(point);
    }
}