using PrototypeSubMod.LightDistortionField;
using UnityEngine;

namespace PrototypeSubMod.UI;

internal class ToggleCloakButton : MonoBehaviour
{
    [SerializeField] private CloakEffectHandler cloakHandler;

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
            main.SetText(HandReticle.TextType.Hand, "ToggleCloakButton", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        }
    }

    public void OnClick()
    {
        cloakHandler.SetUpgradeActive(!cloakHandler.GetUpgradeActive());
    }
}
