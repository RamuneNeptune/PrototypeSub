using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Story;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(VoiceNotification))]
internal class VoiceNotification_Patches
{
    [HarmonyPatch(nameof(VoiceNotification.Play)), HarmonyPatch(new[] { typeof(object[]) }), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Player_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var method = AccessTools.Method(typeof(FMODUWE), nameof(FMODUWE.PlayOneShot), new[] { typeof(FMODAsset), typeof(Vector3), typeof(float) });
        var match = new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == method);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(-1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(GetPlayPosition));

        return matcher.InstructionEnumeration();
    }

    public static Vector3 GetPlayPosition(Vector3 original, VoiceNotification instance)
    {
        if (instance.GetComponent<AttachNotifToPlayer>()) return Player.main.transform.position;

        return original;
    }
}
