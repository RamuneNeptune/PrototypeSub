using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ProtoSaveStateManager : MonoBehaviour
{
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
        }
    }

    private void OnDisable()
    {
        Plugin.GlobalSaveData.prototypePresent = false;
    }

    public bool SubDestroyed() => Plugin.GlobalSaveData.prototypeDestroyed;
}
