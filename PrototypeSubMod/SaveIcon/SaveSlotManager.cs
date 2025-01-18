using Nautilus.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PrototypeSubMod.SaveIcon;

internal static class SaveSlotManager
{
    /// <summary>
    /// Checks if the given save slot contains the specified SaveDataCache 
    /// </summary>
    /// <typeparam name="T">The save data type</typeparam>
    /// <param name="slotName">The name of the save slot to chech. I.e. slot0005</param>
    /// <param name="dataFilePath">The file path of the save data. I.e. MyCoolMod\MyCoolMod.json</param>
    /// <param name="onComplete">The action to call when the check is complete</param>
    /// <returns></returns>
    public static void SaveSlotContainsData<T>(string slotName, string dataFilePath, Action<bool, T> onComplete) where T : SaveDataCache
    {
        UWE.CoroutineHost.StartCoroutine(SaveSlotContainsDataAsync(slotName, dataFilePath, onComplete));
    }

    private static IEnumerator SaveSlotContainsDataAsync<T>(string slotName, string dataFilePath, Action<bool, T> onComplete) where T : SaveDataCache
    {
        var storage = PlatformUtils.main.GetUserStorage();
        var loadOp = storage.LoadFilesAsync(slotName, new List<string>() { dataFilePath });

        yield return new WaitUntil(() => loadOp.done);

        bool success = loadOp.files.TryGetValue(dataFilePath, out var bytes) && bytes != null;
        T saveData = null;
        if (success)
        {
            saveData = JsonConvert.DeserializeObject<T>(Encoding.Default.GetString(bytes));
        }

        onComplete(success, saveData);
    }
}
