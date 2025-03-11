﻿using PrototypeSubMod.Interfaces;
using PrototypeSubMod.UI.AbilitySelection;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal abstract class ProtoUpgrade : MonoBehaviour, IProtoUpgrade, IAbilityIcon
{
    public DummyTechType techType;
    public GameObject[] enableWithInstallation;
    public bool installedAtStart;

    protected bool upgradeEnabled;
    protected bool upgradeInstalled;
    protected bool upgradeLocked;

    public virtual bool GetUpgradeEnabled() => upgradeEnabled;
    public virtual bool GetUpgradeInstalled() => upgradeInstalled;

    public TechType GetTechType() => techType.TechType;

    public virtual void SetUpgradeEnabled(bool enabled)
    {
        if (upgradeLocked) return;

        upgradeEnabled = enabled;
    }

    public virtual void SetUpgradeInstalled(bool installed)
    {
        if (upgradeLocked) return;

        upgradeInstalled = installed;
        foreach (var item in enableWithInstallation)
        {
            if (item == null) continue;

            item.SetActive(installed);
        }

        if (!installed)
        {
            upgradeEnabled = false;
        }
    }

    public virtual void SetUpgradeLocked(bool locked)
    {
        upgradeLocked = locked;
    }

    private void Start()
    {
        if (installedAtStart)
        {
            KnownTech.Add(techType.TechType);
            upgradeInstalled = true;
        }
    }

    public abstract void OnActivated();
    public abstract void OnSelectedChanged(bool changed);

    public virtual bool GetShouldShow()
    {
        return upgradeInstalled;
    }

    public virtual bool GetActive()
    {
        return upgradeEnabled;
    }

    public virtual Sprite GetSprite()
    {
        var atlasSprite = SpriteManager.Get(techType.TechType);
        return SpriteConverter.SpriteFromAtlasSprite(atlasSprite);
    }
}
