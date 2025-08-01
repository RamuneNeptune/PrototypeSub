﻿using HarmonyLib;
using PrototypeSubMod.MotorHandler;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(SubRoot))]
internal class SubRoot_Patches
{
    [HarmonyPatch(nameof(SubRoot.UpdatePowerRating)), HarmonyPostfix]
    private static void UpdatePowerRating_Postfix(SubRoot __instance)
    {
        var motorHandler = __instance.GetComponentInChildren<ProtoMotorHandler>(true);
        if (!motorHandler) return;

        __instance.currPowerRating *= motorHandler.GetEfficiencyMultiplier();
    }

    [HarmonyPatch(nameof(SubRoot.OnPlayerEntered)), HarmonyPrefix]
    private static void OnPlayerEnter_Prefix(SubRoot __instance)
    {
        if (!__instance.voiceNotificationManager) return;
        
        __instance.voiceNotificationManager.ClearQueue();
    }
}
