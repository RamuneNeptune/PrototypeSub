namespace PrototypeSubMod.Compatibility;

internal static class TRPCompatManager
{
    public static bool TRPInstalled
    {
        get
        {
            if (!_trpInitted)
            {
                _trpInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.lee23.theredplague");
                _trpInitted = true;
            }

            return _trpInstalled;
        }
    }

    private static bool _trpInitted;
    private static bool _trpInstalled;
}
