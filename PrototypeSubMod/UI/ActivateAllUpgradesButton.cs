using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.UI;

internal class ActivateAllUpgradesButton : MonoBehaviour
{
    [SerializeField] private SubRoot subRoot;

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
            main.SetText(HandReticle.TextType.Hand, "ActivateAllUpgrades_DEBUG", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.LeftHand);
        }
    }

    public void OnClick()
    {
        foreach (var upgrade in subRoot.GetComponentsInChildren<IProtoUpgrade>(true))
        {
            upgrade.SetUpgradeActive(true);
        }
    }
}
