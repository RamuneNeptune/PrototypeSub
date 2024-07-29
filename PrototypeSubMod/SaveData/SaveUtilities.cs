using SubLibrary.SaveData;

namespace PrototypeSubMod.SaveData;

internal static class SaveUtilities
{
    public static PrototypeSaveData EnsureAsPrototypeData(this BaseSubDataClass baseData)
    {
        var data = baseData as PrototypeSaveData;
        if (data == null) data = new PrototypeSaveData();

        return data;
    }
}
