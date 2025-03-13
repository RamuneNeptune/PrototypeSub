using PrototypeSubMod.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ProtoSaveStateManager : MonoBehaviour
{
    [SaveStateReference]
    public static List<ProtoSaveStateManager> DestroyedManagers;

    [SerializeField] private SubRoot root;

    private void Start()
    {
        root.gameObject.SetActive(!Plugin.GlobalSaveData.prototypeDestroyed);
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
}
