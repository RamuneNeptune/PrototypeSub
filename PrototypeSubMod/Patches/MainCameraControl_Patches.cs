using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MainCameraControl))]
internal class MainCameraControl_Patches
{
    private static bool _overwrite;
    private static Vector2 _overwrittenDelta;

    [HarmonyPatch(nameof(MainCameraControl.OnUpdate)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var getLookDelta = typeof(GameInput).GetMethod("GetLookDelta", BindingFlags.Public | BindingFlags.Static);
        var match = new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == getLookDelta);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(OverwriteLookDelta));

        return matcher.InstructionEnumeration();
    }

    public static Vector2 OverwriteLookDelta(Vector2 originalDelta)
    {
        if (!_overwrite) return originalDelta;

        _overwrittenDelta = originalDelta;
        return Vector2.zero;
    }
}
