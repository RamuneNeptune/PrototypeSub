using UnityEngine;

namespace PrototypeSubMod.SaveIcon;

internal class ProtoSaveIconRegisterer : MonoBehaviour
{
    private void OnEnable()
    {
        Plugin.GlobalSaveData.prototypePresent = true;
    }

    private void OnDisable()
    {
        Plugin.GlobalSaveData.prototypePresent = false;
    }
}
