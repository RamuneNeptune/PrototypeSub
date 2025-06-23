using System;
using Nautilus.Json;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Engine;

public class EngineExteriorEnabledManager : MonoBehaviour
{
    [SerializeField] private GameObject disabledObjects;

    private void Start()
    {
        Plugin.GlobalSaveData.OnStartedSaving += SaveStatus;

        disabledObjects.SetActive(!Plugin.GlobalSaveData.insideEngineFacility);
    }

    private void SaveStatus(object sender, JsonFileEventArgs args)
    {
        Plugin.GlobalSaveData.insideEngineFacility = !disabledObjects.activeSelf;
    }

    private void OnDestroy()
    {
        Plugin.GlobalSaveData.OnStartedSaving -= SaveStatus;
    }
}