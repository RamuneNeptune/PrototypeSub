using Nautilus.Commands;
using PrototypeSubMod.Facilities.Hull;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.Commands;

internal static class PrototypeCommands
{
    private static readonly Vector3 SafePos = new Vector3(-633, -65, -79);
    private const int SafeXOffset = 20;

    [ConsoleCommand("unstuckprototype")]
    public static string UnstuckPrototype()
    {
        var upgradeManagers = GameObject.FindObjectsOfType<ProtoUpgradeManager>();
        for (int i = 0; i < upgradeManagers.Length; i++)
        {
            var sub = upgradeManagers[i].gameObject;
            sub.transform.position = SafePos + new Vector3(SafeXOffset * i, 0, 0);
            sub.transform.rotation = Quaternion.identity;
        }

        return string.Empty;
    }

    [ConsoleCommand("resetwormtimer")]
    public static string ResetWormTimer()
    {
        ErrorMessage.AddError("Proto worm timer reset!");
        WormSpawnEvent.ResetSpawnTimer();
        return string.Empty;
    }

    [ConsoleCommand("protoscreenshot")]
    public static string Screenshot(string path, int superSize)
    {
        ScreenCapture.CaptureScreenshot(path, superSize);
        return string.Empty;
    }
    
    [ConsoleCommand("clearwaitscreen")]
    public static string ClearWaitScreen()
    {
        if (Plugin.prefabLoadWaitItem == null) return string.Empty;

        WaitScreen.Remove(Plugin.prefabLoadWaitItem);
        return string.Empty;
    }
}
