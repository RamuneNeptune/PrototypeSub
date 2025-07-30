using System;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Engine;

public class EngineFacilityRepairPoint : MonoBehaviour
{
    public const int REPAIR_POINTS_COUNT = 4;

    [SerializeField] private FMODAsset[] remainingPointsVoicelines;
    [SerializeField] private FMOD_CustomEmitter onAllSealedSfx;

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
        if (remainingPoints <= 0)
        {
            onAllSealedSfx.Play();
        }
    }
}