using HarmonyLib;
using Story;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Subtitles))]
public class Subtitles_Patches
{
    [HarmonyPatch(nameof(Subtitles.Add)), HarmonyPrefix]
    private static void Add_Prefix(ref string key)
    {
        UpdateVoicelineText(ref key);
    }

    public static void UpdateVoicelineText(ref string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        if (!text.StartsWith("Proto_")) return;

        string[] splits = text.Split('_');
        string ending = splits[splits.Length - 1];
        
        bool endingIsData = ending.Contains("Orion");
        bool goalComplete = StoryGoalManager.main.IsGoalComplete("OrionEndeavorsEncy");
        bool orionNameKnown = PDAEncyclopedia.ContainsEntry("ProtoBuildTerminalEncy");
        bool orIonCheck = (goalComplete && ending != "OrionFullData") || (!goalComplete && ending != "OrionNoData");
        bool unknownCheck = !orionNameKnown && ending != "_OrionUnknown";
        
        if (!endingIsData || orIonCheck || unknownCheck)
        {
            text = text.Replace("_OrionNoData", string.Empty);
            text = text.Replace("_OrionFullData", string.Empty);
            text = text.Replace("_OrionUnknown", string.Empty);
            string suffix = "_OrionNoData";
            if (goalComplete)
            {
                suffix = "_OrionFullData";
            }

            if (!orionNameKnown)
            {
                suffix = "_OrionUnknown";
            }

            text += suffix;
        }
    }
}