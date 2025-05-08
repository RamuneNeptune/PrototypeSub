using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Facilities;

public class SpawnPrecursorDoorway : MonoBehaviour
{
    private PrecursorDoorway precursorDoorway;
    private bool queuedDoorToggle;
    
    private void Start()
    {
        CoroutineHost.StartCoroutine(SpawnGate());
    }

    private IEnumerator SpawnGate()
    {
        var prefabTask = PrefabDatabase.GetPrefabAsync("2d72ad6c-d30d-41be-baa7-0c1dba757b7c");
        yield return prefabTask;
        
        if (!prefabTask.TryGetPrefab(out var keyBarrierPrefab)) throw new Exception("Error retrieving percursor key barrier prefab");
        var instance = Instantiate(keyBarrierPrefab, transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        
        Destroy(instance.GetComponent<LargeWorldEntity>());
        Destroy(instance.GetComponent<PrefabIdentifier>());
        Destroy(instance.transform.Find("DoorSetMotorModeCollider_Walk").gameObject);
        Destroy(instance.transform.Find("DoorSetMotorModeCollider_Swim").gameObject);
        
        precursorDoorway = instance.GetComponent<PrecursorDoorway>();
        if (queuedDoorToggle)
        {
            precursorDoorway.ToggleDoor(queuedDoorToggle);
        }
    }

    public void SetDoorState(bool open)
    {
        if (!precursorDoorway)
        {
            queuedDoorToggle = open;
            return;
        }
        
        precursorDoorway.ToggleDoor(open);
    }
}