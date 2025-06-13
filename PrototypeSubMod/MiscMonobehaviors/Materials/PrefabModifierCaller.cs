using SubLibrary.Monobehaviors;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class PrefabModifierCaller : MonoBehaviour, IProtoEventListener
{
    private bool eventsCalled;
    
    private void Awake()
    {
        CallEvents();
    }

    private void CallEvents()
    {
        if (eventsCalled) return;

        Plugin.Logger.LogInfo($"Calling prefab modifiers");
        
        foreach (var modifier in gameObject.GetComponentsInChildren<PrefabModifier>(true))
        {
            modifier.OnAsyncPrefabTasksCompleted();
            modifier.OnLateMaterialOperation();
        }

        eventsCalled = true;
    }

    public void OnProtoSerialize(ProtobufSerializer serializer) { }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        CallEvents();
    }
}