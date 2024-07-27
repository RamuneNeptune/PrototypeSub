using SubLibrary.SaveData;

namespace PrototypeSubMod.SaveData;

internal static class SaveUtilities
{
    public static PrototypeSaveData EnsurePrototypeData(this BaseSubDataClass baseData)
    {
        var data = baseData as PrototypeSaveData;
        if(data == null) data = new PrototypeSaveData();

        return data;
    }
}
