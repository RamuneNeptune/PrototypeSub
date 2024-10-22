using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.Monobehaviors;

// Eld don't kill me for this one please
internal class AnimatorEventRelay : MonoBehaviour
{
    public UnityEvent onEventTriggered;

    public void TriggerEvent()
    {
        onEventTriggered?.Invoke();
    }
}
