using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.MiscMonobehaviors.Animation;

// Eld don't kill me for this one please
internal class AnimatorEventRelay : MonoBehaviour
{
    public UnityEvent onEventTriggered;

    public void TriggerEvent()
    {
        onEventTriggered?.Invoke();
    }
}
