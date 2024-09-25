using PrototypeSubMod.Interfaces;
using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal class ProtoUpgradeManager : MonoBehaviour, ISaveDataListener
{
    public static ProtoUpgradeManager Instance { get; private set; }

    private Dictionary<TechType, ProtoUpgrade> upgrades = new();
    private PrototypeSaveData saveData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new System.Exception($"More than one ProtoUpgradeManager in the scene! Destroying {this}");
        }

        Instance = this;
    }

    public void SetUpgradeInstalled(TechType techType, bool installed)
    {
        if (!upgrades.TryGetValue(techType, out var upgrade)) throw new System.Exception($"There is no upgrade with the tech type {techType} on the Prototype sub");

        (upgrade as IProtoUpgrade).SetUpgradeInstalled(installed);

        if (saveData.installedModules.Contains(techType) && !installed)
        {
            saveData.installedModules.Remove(techType);
        }
        else if (!saveData.installedModules.Contains(techType) && installed)
        {
            saveData.installedModules.Add(techType);
        }
    }

    public bool GetUpgradeInstalled(TechType techType)
    {
        if (!upgrades.TryGetValue(techType, out var upgrade)) throw new System.Exception($"There is no upgrade with the tech type {techType} on the Prototype sub");

        bool installed = (upgrade as IProtoUpgrade).GetUpgradeInstalled();
        return installed;
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        foreach (var protoUpgrade in GetComponentsInChildren<ProtoUpgrade>(true))
        {
            upgrades.Add(protoUpgrade.techType.TechType, protoUpgrade);
        }

        this.saveData = saveData.EnsureAsPrototypeData();
        Plugin.Logger.LogInfo($"Upgrades count = {upgrades.Count}");
        foreach (var upgrade in upgrades)
        {
            bool installed = this.saveData.installedModules.Contains(upgrade.Key);

            (upgrade.Value as IProtoUpgrade).SetUpgradeInstalled(installed);
        }
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        saveData = this.saveData;
    }

    public List<TechType> GetInstalledUpgrades()
    {
        return upgrades.Keys.ToList();
    }
}
