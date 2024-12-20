using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PingManager))]
internal class PingManager_Patches
{
    [HarmonyPatch(nameof(PingManager.NotifyColor)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> NotifyColor_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new CodeMatch(i => i.opcode == OpCodes.Stloc_1);
        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(CheckForOverrideColor));

        return matcher.InstructionEnumeration();
    }

    public static Color CheckForOverrideColor(Color originalColor, PingInstance instance)
    {
        if (instance.TryGetComponent(out PingColorOverride colorOverride))
        {
            return colorOverride.overrideColor;
        }

        return originalColor;
    }
}
