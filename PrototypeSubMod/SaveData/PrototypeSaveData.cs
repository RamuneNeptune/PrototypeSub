﻿using SubLibrary.SaveData;
using System;
using System.Collections.Generic;

namespace PrototypeSubMod.SaveData;

internal class PrototypeSaveData : ModuleDataClass
{
    //Key: Name of the power source GO | Value: Power source data
    public Dictionary<string, PowerSourceData> powerSourceDatas = new();

    //Key: Equipment slot | Value: Item ID stored in key's slot
    public Dictionary<string, string> serializedPowerEquipment = new();

    //Key: Equipment slot | Value: Item ID stored in key's slot
    public Dictionary<string, string> serializedDeployablesEquipment = new();

    public Dictionary<string, string> serializedPowerAbilityEquipment = new();

    public List<TechType> installedModules = new();

    public Type installedPowerUpgradeType;
    public float currentPowerEffectDuration;
    public int allowedPowerSourceCount;
    public int installedFinCount;

    public struct PowerSourceData
    {
        public bool defaultBatteryCreated;

        public PowerSourceData(bool defaultBatteryCreated)
        {
            this.defaultBatteryCreated = defaultBatteryCreated;
        }
    }
}
