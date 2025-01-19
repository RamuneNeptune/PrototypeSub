using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(UWE.Utils))]
internal class Utils_Patches
{
    [HarmonyPatch(nameof(UWE.Utils.GetEntityRoot)), HarmonyPostfix]
    private static void GetEntityRoot_Postfix(GameObject go, ref GameObject __result)
    {
        if (!go.GetComponent<MarkAsEntityRoot>()) return;

        __result = go;
    }
}
