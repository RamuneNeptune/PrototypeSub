using HarmonyLib;
using PrototypeSubMod.Facilities.Defense;
using PrototypeSubMod.LightDistortionField;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Teleporter;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Player))]
internal class Player_Patches
{
    public static GameObject DummyLDFTarget;

    [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
    private static void Start_Postfix()
    {
        Camera.main.gameObject.AddComponent<LightDistortionApplier>();
        Camera.main.gameObject.AddComponent<ProtoScreenTeleporterFXManager>();
        Camera.main.gameObject.AddComponent<CloakCutoutApplier>();

        DummyLDFTarget = new GameObject("DummyLDFPlayerTarget");
    }

    [HarmonyPatch(nameof(Player.CanEject)), HarmonyPostfix]
    private static void CanEject_Postfix(Player __instance, ref bool __result)
    {
        if (__instance.teleportingLoopSound.playing || ProtoEmergencyWarp.isCharging)
        {
            __result = false;
        }
    }

    [HarmonyPatch(nameof(Player.CheckTeleportationComplete)), HarmonyPostfix]
    private static void CheckForTeleportationComplete_Postfix()
    {
        if (!LargeWorldStreamer.main.IsWorldSettled()) return;

        UWE.CoroutineHost.StartCoroutine(ResetTeleporterColors());
    }

    private static IEnumerator ResetTeleporterColors()
    {
        yield return new WaitForSeconds(3f);

        var teleportManager = Camera.main.GetComponent<ProtoScreenTeleporterFXManager>();
        teleportManager.ResetColors();
    }

    [HarmonyPatch(nameof(Player.CalculateBiome)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CalculateBiome_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var outOfWaterField = typeof(Player).GetField("precursorOutOfWater", BindingFlags.Public | BindingFlags.Instance);
        var match = new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == outOfWaterField);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(BiomeIsWalkableBlacklisted));

        return matcher.InstructionEnumeration();
    }

    public static bool BiomeIsWalkableBlacklisted(bool previousResult, string currentBiome)
    {
        if (currentBiome != Plugin.DEFENSE_CHAMBER_BIOME_NAME) return previousResult;

        return false;
    }

    private const int TUNNEL_REQ_OX = 68;
    private const int MIN_OX_REQ = 135;

    [HarmonyPatch(nameof(Player.GetOxygenPerBreath)), HarmonyPostfix]
    private static void GetOxygenPerBreath_Postfix(Player __instance, ref float __result)
    {
        var biome = __instance.GetBiomeString();
        bool inTunnel = biome.StartsWith("protodefensetunnel");

        if (!inTunnel) return;

        float oxCapacity = __instance.oxygenMgr.GetOxygenCapacity();

        if (oxCapacity < MIN_OX_REQ) return;

        int depth = __instance.depthClass.value;

        bool hasRebreather = Inventory.main.equipment.GetCount(TechType.Rebreather) > 0;
        float depthMultiplier = 1;
        if (!hasRebreather)
        {
            depthMultiplier = 1 + (depth - 1) * 0.5f * 1.75f;
        }

        if (oxCapacity / depthMultiplier < TUNNEL_REQ_OX) return;

        float multiplier = oxCapacity / TUNNEL_REQ_OX / depthMultiplier;

        __result *= multiplier;
    }
}
