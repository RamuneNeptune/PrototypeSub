using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class RepairPointManager : MonoBehaviour
{
    public event Action<Transform> onRepairPointCreated;
    public event Action<Transform> onRepairPointRepaired;

    [SerializeField] private Transform damagePointsParent;
    [SerializeField] private Transform repairPointsParent;

    private Queue<Transform> availableRepairPoints = new();
    private List<Transform> assignedPoints = new();

    private void Start()
    {
        RefreshPointsList();
    }

    public Transform GetRepairPoint()
    {
        var point = availableRepairPoints.Dequeue();
        assignedPoints.Add(point);
        return point;
    }

    private void RefreshPointsList()
    {
        availableRepairPoints.Clear();

        int index = -1;
        foreach (Transform child in damagePointsParent)
        {
            index++;
            if (!child.gameObject.activeSelf) continue;

            if (assignedPoints.Contains(child)) continue;

            availableRepairPoints.Enqueue(repairPointsParent.GetChild(index));
        }
    }

    public void OnDamagePointCreated(Transform point)
    {
        RefreshPointsList();
        onRepairPointCreated(point);
    }

    public void OnDamagePointRepaired(Transform point)
    {
        RefreshPointsList();
        onRepairPointRepaired(point);
        assignedPoints.Remove(point);
    }
}