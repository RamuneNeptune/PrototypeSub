using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class GenericClickTrigger : MonoBehaviour, IHandTarget
{
    [SerializeField] private UnityEvent onClick;

    public void OnHandClick(GUIHand hand)
    {
        onClick?.Invoke();
    }

    public void OnHandHover(GUIHand hand)
    {

    }
}
