using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_LogEntry))]
public class uGUI_LogEntry_Patches
{
    [HarmonyPatch(nameof(uGUI_LogEntry.UpdateText)), HarmonyPrefix]
    private static void UpdateText_Prefix(uGUI_LogEntry __instance)
    {
        Subtitles_Patches.UpdateVoicelineText(ref __instance.entryKey);
    }
}