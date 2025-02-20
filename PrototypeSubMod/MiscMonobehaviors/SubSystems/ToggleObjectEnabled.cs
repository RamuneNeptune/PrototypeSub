using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

internal class ToggleObjectEnabled : MonoBehaviour
{
    public void ToggleEnabled()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
