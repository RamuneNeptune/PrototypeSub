using UnityEngine;

namespace PrototypeSubMod.DestructionEvent;

internal class ProtoSaveStateManager : MonoBehaviour
{
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
}
