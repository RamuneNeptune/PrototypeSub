﻿using PrototypeSubMod.DestructionEvent;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Utility;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.SubTerminal;

internal class SubReconstructionManager : MonoBehaviour
{
    [SerializeField] private ProtoBuildTerminal buildTerminal;

    public GameObject GetSubObject()
    {
        if (ProtoSaveStateManager.DestroyedManagers.Count == 0) return null;

        return ProtoSaveStateManager.DestroyedManagers[0].GetSubRoot();
    }

    public void OnConstructionStarted(Vector3 spawnPos, Quaternion spawnRotation)
    {
        var subTransform = GetSubObject().transform;
        subTransform.position = spawnPos;
        subTransform.rotation = spawnRotation;

        subTransform.gameObject.SetActive(true);
        subTransform.GetComponent<LiveMixin>().ResetHealth();

        int index = 0;
        foreach (var source in subTransform.GetComponentsInChildren<PrototypePowerSource>())
        {
            CoroutineHost.StartCoroutine(source.SpawnDefaultBattery());
            index++;

            if (index > 1) break;
        }
        
        Plugin.GlobalSaveData.prototypeDestroyed = false;
    }

    public void ReconstructSub()
    {
        buildTerminal.RebuildSub();
    }
}
