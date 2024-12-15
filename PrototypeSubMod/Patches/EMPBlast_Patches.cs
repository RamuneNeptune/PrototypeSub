﻿using HarmonyLib;
using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(EMPBlast))]
internal class EMPBlast_Patches
{
    [HarmonyPatch(nameof(EMPBlast.OnTouch)), HarmonyPostfix]
    private static void OnTouch_Postfix(EMPBlast __instance, Collider collider)
    {
        if (collider.attachedRigidbody == null) return;

        GameObject gameObject = collider.attachedRigidbody.gameObject;
        if (!gameObject || !__instance.isValidTarget(gameObject)) return;

        var powerSource = gameObject.GetComponentInChildren<PrototypePowerSource>();
        if (powerSource)
        {
            powerSource.DisableElectronicsForTime(__instance.disableElectronicsTime);
        }
    }
}
