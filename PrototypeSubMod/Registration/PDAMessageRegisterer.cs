using PrototypeSubMod.Patches;

namespace PrototypeSubMod.Registration;

internal class PDAMessageRegisterer
{
    public static void Register()
    {
        PDALog_Patches.entries.Add(("PDA_InterceptorUnlock", "OnInterceptorTestDataDownloaded"));
        PDALog_Patches.entries.Add(("PDA_OnDisableCloak", "OnDefenseCloakDisabled"));
        PDALog_Patches.entries.Add(("PDA_OnEnterMoonpool", "OnEnterDefenseMoonpool"));
        PDALog_Patches.entries.Add(("PDA_OnMoonpoolDisallow", "OnMoonpoolNoPrototype"));
        PDALog_Patches.entries.Add(("PDA_ApproachDefense", "OnApproachDefenseFacility"));
    }
}
