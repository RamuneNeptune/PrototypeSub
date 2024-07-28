using SubLibrary.SaveData;
using System.Collections.Generic;

namespace PrototypeSubMod.SaveData;

internal class PrototypeSaveData : ModuleDataClass
{
    //Key: Name of the power source GO | Value: Power source data
    public Dictionary<string, PowerSourceData> powerSourceDatas = new();

    //Key: Equipment slot | Value: Item ID stored in key's slot
    public Dictionary<string, string> serializedPowerEquipment = new();

    //Key: Prefab identifier ID | Value: Normalized battery charge
    public Dictionary<string, float> normalizedBatteryCharges = new();

    public struct PowerSourceData
    {
        public bool defaultBatteryCreated;

        public PowerSourceData(bool defaultBatteryCreated)
        {
            this.defaultBatteryCreated = defaultBatteryCreated;
        }
    }
}
