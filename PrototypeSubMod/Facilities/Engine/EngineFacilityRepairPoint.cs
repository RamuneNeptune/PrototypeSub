using System;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Engine;

public class EngineFacilityRepairPoint : MonoBehaviour
{
    private const int REPAIR_POINTS_COUNT = 4;

    [SerializeField] private FMODAsset[] remainingPointsVoicelines;

    private void Start()
    {
        var identifier = GetComponent<PrefabIdentifier>();
        if (Plugin.GlobalSaveData.repairedEngineFacilityPoints.Contains(identifier.Id))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnRepair()
    {
        var identifier = GetComponent<PrefabIdentifier>();
        Plugin.GlobalSaveData.repairedEngineFacilityPoints.Add(identifier.Id);
        int remainingPoints = REPAIR_POINTS_COUNT - Plugin.GlobalSaveData.repairedEngineFacilityPoints.Count;
        PDALog.Add(remainingPointsVoicelines[remainingPoints].path);
        gameObject.SetActive(false);
    }
}