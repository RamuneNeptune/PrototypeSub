using UnityEngine;

namespace PrototypeSubMod.Extensions;

internal static class GameObjectExtenstions
{
    public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        GameObject.Destroy(component);
    }

    public static void RemoveComponentImmediate<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        GameObject.DestroyImmediate(component);
    }
}
