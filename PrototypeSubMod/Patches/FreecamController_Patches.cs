﻿using System;
using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(FreecamController))]
public class FreecamController_Patches
{
    public static event Action resetWaterLevel;
    
    [HarmonyPatch(nameof(FreecamController.FreecamToggle)), HarmonyPostfix]
    private static void FreecamToggle_Postfix(FreecamController __instance)
    {
        if (!__instance.mode)
        {
            resetWaterLevel?.Invoke();
        }
    }
}