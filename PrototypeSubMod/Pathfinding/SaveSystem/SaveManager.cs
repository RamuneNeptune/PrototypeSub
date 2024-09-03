using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PrototypeSubMod.Pathfinding.SaveSystem;

public static class SaveManager
{
    private const string PROGRESS_STATUS_HEADER = "Serialization Progress";

    public static void SerializeObject(object obj, string filePath)
    {
        FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);
        BinaryFormatter formatter = new BinaryFormatter();

        formatter.Serialize(stream, obj);

        stream.Close();
    }

    public static T DeserializeObject<T>(string filePath)
    {
        FileStream stream = new FileStream(filePath, FileMode.Open);
        BinaryFormatter formatter = new BinaryFormatter();

        var obj = formatter.Deserialize(stream);

        stream.Close();

        return (T)obj;
    }

    public static T DeserializeObject<T>(byte[] bytes)
    {
        MemoryStream stream = new MemoryStream(bytes);

        BinaryFormatter formatter = new BinaryFormatter();

        var obj = formatter.Deserialize(stream);

        stream.Close();

        return (T)obj;
    }

    private static List<object> GetItemsInArray(Array array, Action<object> forEachObject)
    {
        List<object> items = new List<object>();
        for (int i = 0; i < array.Length; i++)
        {
            int[] indices = GetIndices(array, i);

            items.Add(array.GetValue(indices));
            forEachObject(array.GetValue(indices));
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar(PROGRESS_STATUS_HEADER, "Getting values in array", (float)i / array.Length);
#endif
        }

        return items;
    }

    private static int[] GetIndices(Array array, int index)
    {
        int[] indices = new int[array.Rank];

        for (int i = 0; i < array.Rank; i++)
        {
            int length = array.GetLength(i);
            indices[i] = index % length;
            index /= length;
        }

        return indices;
    }
}