using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class ProtoTerminalHandTarget : MonoBehaviour
{
    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, primaryTooltip, true, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, secondaryTooltip, true, GameInput.Button.None);
        HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnHandClick(GUIHand hand)
    {
        if (informGameObject)
        {
            informGameObject.SendMessage("OnStoryHandTarget", SendMessageOptions.DontRequireReceiver);
        }
        Destroy(destroyGameObject);
    }

    public string primaryTooltip;
    public string secondaryTooltip;

    public GameObject informGameObject;
    public GameObject destroyGameObject;
}
