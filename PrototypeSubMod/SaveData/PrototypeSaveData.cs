using SubLibrary.SaveData;
using System.Collections.Generic;

namespace PrototypeSubMod.SaveData;

internal class PrototypeSaveData : ModuleDataClass
{
    //The key is the name of the power source gameobject
    public Dictionary<string, PowerSourceData> powerSourceDatas = new();

    public struct PowerSourceData
    {
        public bool defaultBatteryCreated;

        public PowerSourceData(bool defaultBatteryCreated)
        {
            this.defaultBatteryCreated = defaultBatteryCreated;
        }
    }
}
