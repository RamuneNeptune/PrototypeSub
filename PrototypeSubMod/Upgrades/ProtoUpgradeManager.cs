using PrototypeSubMod.Interfaces;
using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System.Collections.Generic;
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
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        foreach (var protoUpgrade in GetComponentsInChildren<ProtoUpgrade>(true))
        {
            upgrades.Add(protoUpgrade.techType.TechType, protoUpgrade);
        }
    }

    public void SetUpgradeInstalled(TechType techType, bool installed)
    {
        if (!upgrades.TryGetValue(techType, out var upgrade)) throw new System.Exception($"There is no upgrade with the tech type {techType} on the Prototype sub");

        (upgrade as IProtoUpgrade).SetUpgradeInstalled(installed);
        ErrorMessage.AddMessage($"Upgrade '{techType}' set installed to {installed}");

        if (saveData.installedModules.Contains(techType) && !installed)
        {
            saveData.installedModules.Remove(techType);
        }
        else if (!saveData.installedModules.Contains(techType) && installed)
        {
            saveData.installedModules.Add(techType);
        }
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        this.saveData = saveData.EnsureAsPrototypeData();
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
}
