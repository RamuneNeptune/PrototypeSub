using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.UI;

internal class ToggleUpgradeButton : MonoBehaviour
{
    [SerializeField] private IProtoUpgrade upgrade;

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
            string prompt = upgrade.GetUpgradeActive() ? "Deactivate" : "Activate";
            main.SetText(HandReticle.TextType.Hand, $"{prompt} {upgrade.GetUpgradeName()}", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        }
    }

    public void OnClick()
    {
        upgrade.SetUpgradeActive(!upgrade.GetUpgradeActive());
    }
}
