using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.UI;

internal class ToggleUpgradeButton : MonoBehaviour
{
    [SerializeField] private Component upgrade;
    private IProtoUpgrade protoUpgrade;

    private bool hovering;

    private void OnValidate()
    {
        if (upgrade != null && upgrade is IProtoUpgrade)
        {
            protoUpgrade = (IProtoUpgrade)upgrade;
            return;
        }

        protoUpgrade = upgrade.GetComponentInChildren<IProtoUpgrade>();
        if (protoUpgrade == null)
        {
            Debug.LogError($"Invalid component. Proto upgrade required");
            upgrade = null;
        }
    }

    private void Start()
    {
        if (protoUpgrade != null) return;

        if(upgrade is IProtoUpgrade)
        {
            protoUpgrade = (IProtoUpgrade)upgrade;
            return;
        }

        protoUpgrade = upgrade.GetComponentInChildren<IProtoUpgrade>();
    }

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
            string prompt = protoUpgrade.GetUpgradeInstalled() ? "Deactivate" : "Activate";
            main.SetText(HandReticle.TextType.Hand, $"{prompt} {protoUpgrade.GetUpgradeName()}", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        }
    }

    public void OnClick()
    {
        protoUpgrade.SetUpgradeInstalled(!protoUpgrade.GetUpgradeInstalled());
    }
}
