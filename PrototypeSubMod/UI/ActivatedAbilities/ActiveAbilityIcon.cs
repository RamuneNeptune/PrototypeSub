using PrototypeSubMod.UI.AbilitySelection;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.ActivatedAbilities;

internal class ActiveAbilityIcon : MonoBehaviour
{
    [SerializeField] private Image image;

    private IAbilityIcon icon;

    public void SetIcon(IAbilityIcon icon)
    { 
        image.sprite = icon.GetSprite();
        this.icon = icon;
    }

    public IAbilityIcon GetIcon()
    {
        return icon;
    }
}
