using HarmonyLib;
using PrototypeSubMod.Facilities.Defense;
using PrototypeSubMod.LightDistortionField;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Teleporter;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Player))]
internal class Player_Patches
{
    [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
    private static void Start_Postfix()
    {
        Camera.main.gameObject.AddComponent<LightDistortionApplier>();
        Camera.main.gameObject.AddComponent<ProtoScreenTeleporterFXManager>();
        Camera.main.gameObject.AddComponent<CloakCutoutApplier>();
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
}
