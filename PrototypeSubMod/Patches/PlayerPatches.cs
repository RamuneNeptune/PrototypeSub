using HarmonyLib;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Player))]
internal class PlayerPatches
{
    public static Vector3[] lastPlayerPositions = new Vector3[5];
    private static int playerPosIndex;

    [HarmonyPatch(nameof(Player.FixedUpdate)), HarmonyPostfix]
    private static void FixedUpdate_Postfix(Player __instance)
    {
        lastPlayerPositions[playerPosIndex] = __instance.transform.position;
        playerPosIndex = (playerPosIndex + 1) % lastPlayerPositions.Length;
    }
}
