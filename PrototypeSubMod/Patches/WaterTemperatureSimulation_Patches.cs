using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(WaterTemperatureSimulation))]
internal class WaterTemperatureSimulation_Patches
{
    [HarmonyPatch(nameof(WaterTemperatureSimulation.GetTemperature)), HarmonyPatch(new[] { typeof(Vector3) }), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GetTemperature_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var magnitude = typeof(Vector3).GetProperty("magnitude", BindingFlags.Public | BindingFlags.Instance).GetGetMethod();
        var match = new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == magnitude);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 6))
            .InsertAndAdvance(Transpilers.EmitDelegate(GetMagnitudeMultiplier));

        return matcher.InstructionEnumeration();
    }

    public static float GetMagnitudeMultiplier(float originalMagnitude, IEcoTarget ecoTarget)
    {
        var gameObject = ecoTarget.GetGameObject();

        if (!gameObject) return originalMagnitude;

        var distanceMultiplier = gameObject.GetComponent<HeatAreaDistanceMultiplier>();
        if (!distanceMultiplier) return originalMagnitude;

        return originalMagnitude * distanceMultiplier.GetMultiplier();
    }
}
