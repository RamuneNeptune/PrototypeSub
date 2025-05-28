using PrototypeSubMod.Patches;

namespace PrototypeSubMod.Registration;

internal static class PDAMessageRegisterer
{
    public static void Register()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        PDALog_Patches.entries.Add(("PDA_InterceptorUnlock", "OnInterceptorTestDataDownloaded"));
        PDALog_Patches.entries.Add(("PDA_OnDisableCloak", "OnDefenseCloakDisabled"));
        PDALog_Patches.entries.Add(("PDA_OnEnterMoonpool", "OnEnterDefenseMoonpool"));
        PDALog_Patches.entries.Add(("PDA_OnMoonpoolDisallow", "OnMoonpoolNoPrototype"));
        PDALog_Patches.entries.Add(("PDA_ApproachDefense", "OnApproachDefenseFacility"));
        PDALog_Patches.entries.Add(("PDA_NoFireExtinguisher", "NotifyPlayerNoExtinguishers"));
        PDALog_Patches.entries.Add(("PDA_Breach3Left", "PDA_Breach3Left"));
        PDALog_Patches.entries.Add(("PDA_Breach2Left", "PDA_Breach2Left"));
        PDALog_Patches.entries.Add(("PDA_Breach1Left", "PDA_Breach1Left"));
        PDALog_Patches.entries.Add(("PDA_Breach0Left", "PDA_Breach0Left"));

        sw.Stop();
        Plugin.Logger.LogInfo($"PDA Messages registered in {sw.ElapsedMilliseconds}ms");
    }
}
