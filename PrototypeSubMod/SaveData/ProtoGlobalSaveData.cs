using Nautilus.Json;
using PrototypeSubMod.Utility;
using System.Collections.Generic;

namespace PrototypeSubMod.SaveData;

internal class ProtoGlobalSaveData : SaveDataCache
{
    //Key: Prefab identifier ID | Value: Normalized battery charge
    public Dictionary<string, float> normalizedBatteryCharges = new();

    public Dictionary<string, float> deployableLightLifetimes = new();
    public List<ProtoUpgradeCategory> unlockedCategoriesLastCheck = new();

    public bool prototypePresent;

    public bool defenseCloakDisabled;
    public bool moonpoolDoorOpened;
    public bool reactorSequenceComplete;
    public bool defensePingSpawned;
}
