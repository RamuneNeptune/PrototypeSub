using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class ProtoTerminalHandTarget : MonoBehaviour, IHandTarget
{
    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, primaryTooltip, true, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, secondaryTooltip, true, GameInput.Button.None);
        HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnHandClick(GUIHand hand)
    {
        foreach (var item in informGameObjects)
        {
            // Keeping the same name so as not the break events on the original prefab
            item.SendMessage("OnStoryHandTarget");
        }
        Destroy(destroyGameObject);
    }

    public string primaryTooltip;
    public string secondaryTooltip;

    public GameObject[] informGameObjects;
    public GameObject destroyGameObject;
}
