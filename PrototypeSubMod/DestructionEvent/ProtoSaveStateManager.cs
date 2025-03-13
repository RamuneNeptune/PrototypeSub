using PrototypeSubMod.Utility;
using SubLibrary.SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ProtoSaveStateManager : MonoBehaviour, IProtoEventListener
{
    [SaveStateReference]
    public static List<ProtoSaveStateManager> DestroyedManagers = new();

    [SerializeField] private SubRoot root;

    private void Awake()
    {
        if (DestroyedManagers == null) DestroyedManagers = new();

        root.gameObject.SetActive(!Plugin.GlobalSaveData.prototypeDestroyed);
        if (Plugin.GlobalSaveData.prototypeDestroyed && !DestroyedManagers.Contains(this))
        {
            DestroyedManagers.Add(this);
        }
    }

    private void OnEnable()
    {
        if (!Plugin.GlobalSaveData.prototypeDestroyed)
        {
            Plugin.GlobalSaveData.prototypePresent = true;
            DestroyedManagers.Remove(this);
        }
    }

    private void OnDisable()
    {
        Plugin.GlobalSaveData.prototypePresent = false;
        if (Plugin.GlobalSaveData.prototypeDestroyed && !DestroyedManagers.Contains(this))
        {
            DestroyedManagers.Add(this);
        }
    }

    public GameObject GetSubRoot()
    {
        return root.gameObject;
    }

    public bool SubDestroyed() => Plugin.GlobalSaveData.prototypeDestroyed;

    public void OnProtoSerialize(ProtobufSerializer serializer)
    {
        UWE.CoroutineHost.StartCoroutine(SetActiveState());
    }

    public void OnProtoDeserialize(ProtobufSerializer serializer)
    {
        UWE.CoroutineHost.StartCoroutine(SetActiveState());
    }

    private IEnumerator SetActiveState()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        root.gameObject.SetActive(!Plugin.GlobalSaveData.prototypeDestroyed);
    }
}
