using PrototypeSubMod.Registration;
using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_MainMenu))]
public class uGUI_MainMenu_Patches
{
    [HarmonyPatch(nameof(uGUI_MainMenu.LoadGameAsync)), HarmonyPrefix]
    private static void LoadGameAsync_Prefix()
    {
        VoicelineRegisterer.UpdateVoicelines();
    }
}