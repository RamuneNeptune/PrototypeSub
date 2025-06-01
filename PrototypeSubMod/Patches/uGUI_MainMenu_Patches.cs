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

    [HarmonyPatch(nameof(uGUI_MainMenu.LoadGameAsync)), HarmonyPostfix]
    private static void LoadGameAsync_Postfix()
    {
        if (Plugin.easyPrefabsLoaded) return;
        
        Plugin.prefabLoadWaitItem = WaitScreen.Add("ProtoEasyPrefabs");
    }
}