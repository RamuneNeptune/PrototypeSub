using PrototypeSubMod.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI;

internal class ToggleUpgradeButton : MonoBehaviour
{
    [SerializeField] private Component upgrade;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite enabledSprite;
    [SerializeField] private Image image;
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

        if (!image) TryGetComponent(out image);
    }

    private void Start()
    {
        if (protoUpgrade != null) return;

        if (upgrade is IProtoUpgrade)
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
            string prompt = protoUpgrade.GetUpgradeEnabled() ? "ProtoButtonDisable" : "ProtoButtonEnable";
            string text = Language.main.Get(protoUpgrade.GetTechType());

            prompt = Language.main.Get(prompt);
            main.SetText(HandReticle.TextType.Hand, $"{prompt} {text}", true, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        }
    }

    public void ToggleUpgradeEnabled()
    {
        protoUpgrade.SetUpgradeEnabled(!protoUpgrade.GetUpgradeEnabled());
        image.sprite = protoUpgrade.GetUpgradeEnabled() ? enabledSprite : normalSprite;
    }
}
