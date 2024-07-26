using SubLibrary.SaveData;

namespace PrototypeSubMod.SaveData;

internal class PrototypeSaveData : ModuleDataClass
{


    public struct PowerSourceData
    {
        public bool defaultBatteryCreated;

        public PowerSourceData(bool defaultBatteryCreated)
        {
            this.defaultBatteryCreated = defaultBatteryCreated;
        }
    }
}
