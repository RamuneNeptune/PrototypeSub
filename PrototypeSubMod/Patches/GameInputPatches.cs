using HarmonyLib;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(GameInput))]
internal class GameInputPatches
{
    [HarmonyPatch(nameof(GameInput.GetLookDelta)), HarmonyPostfix]
    private static void GetLookDelta_Postfix(ref Vector2 __result)
    {
        if (Player.main.teleportingLoopSound.playing)
        {
            __result = Vector2.zero;
        }
    }
}
