using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.MiscMonobehaviors.Animation;

// Eld don't kill me for this one please
internal class AnimatorEventRelay : MonoBehaviour
{
    public UnityEvent onEventTriggered;
    public UnityEvent onEvent2Triggered;
    public UnityEvent onEvent3Triggered;

    public void TriggerEvent()
    {
        onEventTriggered?.Invoke();
    }

    public void TriggerEvent2()
    {
        onEvent2Triggered?.Invoke();
    }
    
    public void TriggerEvent3()
    {
        onEvent3Triggered?.Invoke();
    }
}
