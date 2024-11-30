using Newtonsoft.Json;
using PrototypeSubMod.SaveData;
using System.IO;

namespace PrototypeSubMod.SaveIcon;

internal static class SaveSlotManager
{
    private readonly static string SubnauticaFolder = Plugin.Assembly.Location.Split(new[] { "Subnautica" }, System.StringSplitOptions.None)[0] + "Subnautica";
    private readonly static string SavesFolder = Path.Combine(SubnauticaFolder, "SNAppData", "SavedGames");

    public static bool SaveContainsProtoData(string slotName, out ProtoGlobalSaveData saveData)
    {
        saveData = null;
        string saveDataPath = Path.Combine(SavesFolder, slotName, "PrototypeSubMod");
        if (!Directory.Exists(saveDataPath)) return false;

        string saveDataJson = File.ReadAllText(Path.Combine(saveDataPath, "PrototypeSubMod.json"));
        try
        {
            saveData = JsonConvert.DeserializeObject<ProtoGlobalSaveData>(saveDataJson);
        }
        catch (System.Exception e)
        {
            Plugin.Logger.LogError($"Problem deserializing prototype data at path {Path.Combine(saveDataPath, "PrototypeSubMod.json")}");
            throw e;
        }

        return true;
    }
}
