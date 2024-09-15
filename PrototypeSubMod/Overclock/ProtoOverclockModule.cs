using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.Overclock;

internal class ProtoOverclockModule : MonoBehaviour, IProtoUpgrade
{
    private bool upgradeInstalled;
    private bool upgradeEnabled;

    public void SetUpgradeInstalled(bool installed)
    {
        upgradeInstalled = installed;
    }

    public void SetUpgradeEnabled(bool enabled)
    {
        upgradeEnabled = enabled;
    }

    public string GetUpgradeName()
    {
        return "Overclock Module";
    }

    public bool GetUpgradeEnabled() => upgradeEnabled;
    public bool GetUpgradeInstalled() => upgradeInstalled;
}
