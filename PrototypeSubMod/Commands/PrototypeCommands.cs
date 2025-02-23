using Nautilus.Commands;
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

        return "";
    }
}
