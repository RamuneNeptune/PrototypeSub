using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class RepairPointManager : MonoBehaviour
{
    public event Action<CyclopsDamagePoint> onRepairPointCreated;
    public event Action<CyclopsDamagePoint> onRepairPointRepaired;

    [SerializeField] private Transform damagePointsParent;
    [SerializeField] private Transform repairPointsParent;

    private Queue<CyclopsDamagePoint> availableRepairPoints = new();
    private List<CyclopsDamagePoint> assignedPoints = new();

    private void Start()
    {
        RefreshPointsList();
    }

    public CyclopsDamagePoint GetRepairPoint()
    {
        var point = availableRepairPoints.Dequeue();
        assignedPoints.Add(point);
        return point;
    }

    private void RefreshPointsList()
    {
        availableRepairPoints.Clear();

        int index = -1;
        foreach (CyclopsDamagePoint point in damagePointsParent.GetComponentsInChildren<CyclopsDamagePoint>())
        {
            index++;
            if (!point.gameObject.activeSelf) continue;

            if (assignedPoints.Contains(point)) continue;

            availableRepairPoints.Enqueue(point);
        }
    }

    public void OnDamagePointCreated(CyclopsDamagePoint point)
    {
        RefreshPointsList();
        onRepairPointCreated(point);
    }

    public void OnDamagePointRepaired(CyclopsDamagePoint point)
    {
        RefreshPointsList();
        onRepairPointRepaired(point);
        assignedPoints.Remove(point);
    }
}