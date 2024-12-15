using HarmonyLib;
using PrototypeSubMod.RepairBots;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsExternalDamageManager))]
internal class DamageManager_Patches
{
    [HarmonyPatch(nameof(CyclopsExternalDamageManager.CreatePoint)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CreatePoint_Postfix(IEnumerable<CodeInstruction> instructions)
    {
        var method = typeof(CyclopsDamagePoint).GetMethod("SpawnFx", BindingFlags.Public | BindingFlags.Instance);
        var match = new CodeMatch(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == method);

        var codeMatcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(TryNotifyPointCreation));

        return codeMatcher.InstructionEnumeration();
    }

    public static void TryNotifyPointCreation(int pointIndex, CyclopsExternalDamageManager damageManger)
    {
        if (!damageManger.TryGetComponent(out RepairPointManager pointManager)) return;

        var newPoint = damageManger.unusedDamagePoints[pointIndex];

        pointManager.OnDamagePointCreated(newPoint);
    }

    [HarmonyPatch(nameof(CyclopsExternalDamageManager.RepairPoint)), HarmonyPostfix]
    private static void RepairPoint_Postfix(CyclopsExternalDamageManager __instance, CyclopsDamagePoint point)
    {
        if (!__instance.TryGetComponent(out RepairPointManager pointManager)) return;

        pointManager.OnDamagePointRepaired(point);
    }
}
