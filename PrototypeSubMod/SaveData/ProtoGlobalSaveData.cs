using Nautilus.Json;
using System.Collections.Generic;

namespace PrototypeSubMod.SaveData;

internal class ProtoGlobalSaveData : SaveDataCache
{
    //Key: Prefab identifier ID | Value: Normalized battery charge
    public Dictionary<string, float> normalizedBatteryCharges = new();

    public bool defenseCloakDisabled;
}
