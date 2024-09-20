using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class RepairPointManager : MonoBehaviour
{
    [SerializeField] private Transform damagePointsParent;
    [SerializeField] private Transform repairPointsParent;

    private Queue<Transform> availableRepairPoints;
    private List<Transform> assignedPoints;

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

    public void OnDamagePointsChanged()
    {
        RefreshPointsList();
    }
}