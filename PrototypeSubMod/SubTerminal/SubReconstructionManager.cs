using PrototypeSubMod.DestructionEvent;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Utility;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.SubTerminal;

internal class SubReconstructionManager : MonoBehaviour
{
    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private DummyTechType reconstructionType;

    public GameObject GetSubObject()
    {
        if (ProtoSaveStateManager.DestroyedManagers.Count == 0) return null;

        return ProtoSaveStateManager.DestroyedManagers[0].GetSubRoot();
    }

    public TechType GetReconstructionTechType()
    {
        return reconstructionType.TechType;
    }

    public void OnConstructionStarted(Vector3 spawnPos, Quaternion spawnRotation)
    {
        Plugin.GlobalSaveData.prototypeDestroyed = false;

        var subTransform = GetSubObject().transform;
        subTransform.position = spawnPos;
        subTransform.rotation = spawnRotation;

        subTransform.gameObject.SetActive(true);
        subTransform.GetComponent<LiveMixin>().ResetHealth();

        foreach (var point in subTransform.GetComponentsInChildren<CyclopsDamagePoint>())
        {
            point.OnRepair();
        }

        foreach (var source in subTransform.GetComponentsInChildren<PrototypePowerSource>())
        {
            CoroutineHost.StartCoroutine(source.SpawnDefaultBattery());
        }
    }

    public void ReconstructSub()
    {
        buildTerminal.RebuildSub(this);
    }
}
