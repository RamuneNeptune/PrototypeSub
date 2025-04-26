using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Utility;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PowerRelay))]
public class PowerRelay_Patches
{
    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ModifyPower_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new[]
        {
            new CodeMatch(i => i.opcode == OpCodes.Ldloc_0),
            new CodeMatch(i => i.opcode == OpCodes.Ret)
        };

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_2))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldind_R4))
            .InsertAndAdvance(Transpilers.EmitDelegate(CallMethodIfNeeded));
        
        return matcher.InstructionEnumeration();
    }
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPowerFromInbound)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ModifyPowerFromInbound_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new[]
        {
            new CodeMatch(i => i.opcode == OpCodes.Ldloc_0),
            new CodeMatch(i => i.opcode == OpCodes.Ret)
        };

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_2))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldind_R4))
            .InsertAndAdvance(Transpilers.EmitDelegate(CallMethodIfNeeded));
        
        return matcher.InstructionEnumeration();
    }

    public static void CallMethodIfNeeded(PowerRelay relay, float modified)
    {
        if (relay.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModifiedPower(modified);
        }
    }
}