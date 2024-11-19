using PrototypeSubMod.Interfaces;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal abstract class ProtoUpgrade : MonoBehaviour, IProtoUpgrade
{
    public DummyTechType techType;
    public GameObject[] enableWithInstallation;
    public bool installedAtStart;

    protected bool upgradeEnabled;
    protected bool upgradeInstalled;

    public virtual bool GetUpgradeEnabled() => upgradeEnabled;
    public virtual bool GetUpgradeInstalled() => upgradeInstalled;

    public TechType GetTechType() => techType.TechType;

    public virtual void SetUpgradeEnabled(bool enabled)
    {
        upgradeEnabled = enabled;
    }

    public virtual void SetUpgradeInstalled(bool installed)
    {
        upgradeInstalled = installed;
        foreach (var item in enableWithInstallation)
        {
            if (item == null) continue;

            item.SetActive(installed);
        }
    }

    private void Start()
    {
        if (installedAtStart)
        {
            KnownTech.Add(techType.TechType);
            upgradeInstalled = true;
        }
    }
}
