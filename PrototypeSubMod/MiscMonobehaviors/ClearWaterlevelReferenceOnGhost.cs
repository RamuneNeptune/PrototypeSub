using System;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class ClearWaterlevelReferenceOnGhost : MonoBehaviour
{
    [SerializeField] private PrecursorMoonPoolTrigger trigger;

    private void OnValidate()
    {
        if (!trigger) TryGetComponent(out trigger);
    }

    private void OnEnable()
    {
        FreecamController_Patches.resetWaterLevel += ForceOnTriggerExit;
    }

    private void OnDisable()
    {
        FreecamController_Patches.resetWaterLevel -= ForceOnTriggerExit;
    }

    private void ForceOnTriggerExit()
    {
        trigger.OnTriggerExit(Player.mainCollider);
        Player.main.SetPrecursorOutOfWater(false);
    }
}