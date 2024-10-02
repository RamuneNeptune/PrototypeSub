using PrototypeSubMod.Interfaces;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal abstract class ProtoUpgrade : MonoBehaviour, IProtoUpgrade
{
    public DummyTechType techType;
    public GameObject[] enableWithInstallation;
    public bool unlockedAtStart;

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
            item.SetActive(installed);
        }
    }

    private void Awake()
    {
        if (!unlockedAtStart) return;

        KnownTech.Add(techType.TechType);
    }
}
