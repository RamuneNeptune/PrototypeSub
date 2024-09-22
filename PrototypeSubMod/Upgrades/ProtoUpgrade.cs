using PrototypeSubMod.Interfaces;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal abstract class ProtoUpgrade : MonoBehaviour, IProtoUpgrade
{
    public DummyTechType techType;
    public bool unlockedAtStart;

    public abstract bool GetUpgradeEnabled();

    public abstract bool GetUpgradeInstalled();

    public abstract string GetUpgradeName();

    public abstract void SetUpgradeEnabled(bool enabled);

    public abstract void SetUpgradeInstalled(bool installed);

    private void Awake()
    {
        if (!unlockedAtStart) return;

        KnownTech.Add(techType.TechType);
    }
}
