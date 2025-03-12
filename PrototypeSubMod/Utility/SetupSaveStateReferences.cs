using System.Collections.Generic;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.Utility;

internal static class SetupSaveStateReferences
{
    private static List<FieldInfo> SaveStateReferences = new();
    private static bool eventAdded;

    public static void SetupReferences(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is SaveStateReferenceAttribute)
                    {
                        SaveStateReferences.Add(field);
                        break;
                    }
                }
            }
        }

        if (!eventAdded)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        eventAdded = true;
        foreach (var info in SaveStateReferences)
        {
            info.SetValue(null, null);
        }
    }
}
