using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class EngineActiveManager : MonoBehaviour
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private GameObject[] engineOnObjects;
    [SerializeField] private GameObject[] engineOffObjects;

    private void Start()
    {
        UpdateObjectStatuses();
    }

    // Called via CyclopsMotorMode.ChangeEngineState using BroadcastMessage 
    public void RecalculateNoiseValues()
    {
        UpdateObjectStatuses();
    }

    private void UpdateObjectStatuses()
    {
        foreach (var obj in engineOnObjects)
        {
            obj.SetActive(motorMode.engineOn);
        }
        
        foreach (var obj in engineOffObjects)
        {
            obj.SetActive(!motorMode.engineOn);
        }
    }
}