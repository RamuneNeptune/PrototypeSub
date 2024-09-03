using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.UI;

internal class GenericUIButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private string localizationKey;

    private bool hovering;

    public void OnMouseEnter()
    {
        hovering = true;
    }

    public void OnMouseExit()
    {
        hovering = false;
    }

    private void Update()
    {
        if (hovering)
        {
            HandReticle main = HandReticle.main;
            main.SetText(HandReticle.TextType.Hand, localizationKey, true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        }
    }

    public void OnClick()
    {
        onClick?.Invoke();
    }
}
