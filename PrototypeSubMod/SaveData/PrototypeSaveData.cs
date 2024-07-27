using SubLibrary.SaveData;
using System.Collections.Generic;

namespace PrototypeSubMod.SaveData;

internal class PrototypeSaveData : ModuleDataClass
{
    //The key is the name of the power source gameobject
    public Dictionary<string, PowerSourceData> powerSourceDatas = new();

    //Key: equipment slot | Value: item ID stored in key's slot
    public Dictionary<string, string> serializedModules = new();

    public struct PowerSourceData
    {
        public bool defaultBatteryCreated;

        public PowerSourceData(bool defaultBatteryCreated)
        {
            this.defaultBatteryCreated = defaultBatteryCreated;
        }
    }
}
