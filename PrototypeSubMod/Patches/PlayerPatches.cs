using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using PrototypeSubMod.Teleporter;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Player))]
internal class PlayerPatches
{
    public static Vector3[] lastPlayerPositions = new Vector3[5];
    private static int playerPosIndex;

    [HarmonyPatch(nameof(Player.Start)), HarmonyPostfix]
    private static void Start_Postfix()
    {
        Camera.main.gameObject.AddComponent<LightDistortionApplier>();
    }

    [HarmonyPatch(nameof(Player.FixedUpdate)), HarmonyPostfix]
    private static void FixedUpdate_Postfix(Player __instance)
    {
        lastPlayerPositions[playerPosIndex] = __instance.transform.position;
        playerPosIndex = (playerPosIndex + 1) % lastPlayerPositions.Length;
    }

    [HarmonyPatch(nameof(Player.CanEject)), HarmonyPostfix]
    private static void CanEject_Postfix(Player __instance, ref bool __result)
    {
        if (__instance.teleportingLoopSound.playing || ProtoEmergencyWarp.isCharging)
        {
            __result = false;
        }
    }
}
