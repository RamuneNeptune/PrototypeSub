using PrototypeSubMod.Interfaces;
using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal class ProtoUpgradeManager : MonoBehaviour, ISaveDataListener
{
    public static ProtoUpgradeManager Instance { get; private set; }

    private List<TechType> InstalledUpgrades
    {
        get
        {
            if (_installedUpgrades == null || upgradesDirty)
            {
                _installedUpgrades = new();
                foreach (var upgrade in upgrades)
                {
                    if ((upgrade.Value as IProtoUpgrade).GetUpgradeInstalled())
                    {
                        _installedUpgrades.Add(upgrade.Key);
                    }
                }
            }

            return _installedUpgrades;
        }
    }

    private List<TechType> _installedUpgrades;
    private bool upgradesDirty;

    private Dictionary<TechType, ProtoUpgrade> upgrades = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new Exception($"More than one ProtoUpgradeManager in the scene! Destroying {this}");
        }

        Instance = this;
    }

    private void Start()
    {
        DevConsole.RegisterConsoleCommand(this, "ToggleUpgradeEnabled");
        DevConsole.RegisterConsoleCommand(this, "ToggleUpgradeInstalled");

        if (upgrades.Count == 0)
        {
            foreach (var protoUpgrade in GetComponentsInChildren<ProtoUpgrade>(true))
            {
                upgrades.Add(protoUpgrade.techType.TechType, protoUpgrade);
            }
        }
    }

    public void SetUpgradeInstalled(TechType techType, bool installed)
    {
        if (!upgrades.TryGetValue(techType, out var upgrade)) throw new Exception($"There is no upgrade with the tech type {techType} on the Prototype sub");

        (upgrade as IProtoUpgrade).SetUpgradeInstalled(installed);

        upgradesDirty = true;
    }

    public bool GetUpgradeInstalled(TechType techType)
    {
        return InstalledUpgrades.Contains(techType);
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        foreach (var protoUpgrade in GetComponentsInChildren<ProtoUpgrade>(true))
        {
            upgrades.Add(protoUpgrade.techType.TechType, protoUpgrade);
        }

        var protoData = saveData.EnsureAsPrototypeData();
        foreach (var upgrade in upgrades)
        {
            bool installed = protoData.installedModules.Contains(upgrade.Key);

            (upgrade.Value as IProtoUpgrade).SetUpgradeInstalled(installed);
        }
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();

        upgradesDirty = true;
        protoData.installedModules = InstalledUpgrades;
        saveData = protoData;
    }

    public List<TechType> GetInstalledUpgrades()
    {
        return InstalledUpgrades;
    }

    public void OnConsoleCommand_toggleupgradeenabled(NotificationCenter.Notification notification)
    {
        if (notification == null)
        {
            ErrorMessage.AddError($"'toggleupgradeenabled' expects an upgrade tech type.");
            return;
        }

        if (notification.data.Count > 1 || notification.data.Count <= 0)
        {
            ErrorMessage.AddError($"Invalid argument count. ToggleUpgradeEnabled expects the tech type for an upgrade");
            return;
        }

        string upgradeTechType = "";
        try
        {
            upgradeTechType = notification.data[0] as string;
        }
        catch (Exception e)
        {
            throw e;
        }

        TechType techType = TechType.None;
        try
        {
            techType = (TechType)Enum.Parse(typeof(TechType), upgradeTechType, true);
        }
        catch (Exception e)
        {
            ErrorMessage.AddError($"Error parsing \"{upgradeTechType}\" as a tech type. Check log for full details.");
            throw e;
        }

        if (!upgrades.TryGetValue(techType, out var upgrade))
        {
            ErrorMessage.AddError($"Upgrade with tech type \"{upgradeTechType}\" is not on the prototype");
            return;
        }

        upgrade.SetUpgradeEnabled(!upgrade.GetUpgradeEnabled());

        ErrorMessage.AddError($"{techType} enabled set to {upgrade.GetUpgradeEnabled()}");
    }

    public void OnConsoleCommand_toggleupgradeinstalled(NotificationCenter.Notification notification)
    {
        if (notification == null)
        {
            ErrorMessage.AddError($"'toggleupgradeinstalled' expects an upgrade tech type.");
            return;
        }

        if (notification.data.Count > 1 || notification.data.Count <= 0)
        {
            ErrorMessage.AddError($"Invalid argument count. ToggleUpgradeInstalled expects the tech type for an upgrade");
            return;
        }

        string upgradeTechType = "";
        try
        {
            upgradeTechType = notification.data[0] as string;
        }
        catch (Exception e)
        {
            throw e;
        }

        TechType techType = TechType.None;
        try
        {
            techType = (TechType)Enum.Parse(typeof(TechType), upgradeTechType, true);
        }
        catch (Exception e)
        {
            ErrorMessage.AddError($"Error parsing \"{upgradeTechType}\" as a tech type. Check log for full details.");
            throw e;
        }

        if (!upgrades.TryGetValue(techType, out var upgrade))
        {
            ErrorMessage.AddError($"Upgrade with tech type \"{upgradeTechType}\" is not on the prototype");
            return;
        }

        upgrade.SetUpgradeInstalled(!upgrade.GetUpgradeInstalled());

        ErrorMessage.AddError($"{techType} installed set to {upgrade.GetUpgradeInstalled()}");
        upgradesDirty = true;
    }
}
