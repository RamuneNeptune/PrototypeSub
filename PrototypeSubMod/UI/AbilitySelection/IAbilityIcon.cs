using UnityEngine;

namespace PrototypeSubMod.UI.AbilitySelection;

internal interface IAbilityIcon
{ 
    public void OnActivated();
    public void OnSelectedChanged(bool changed);
    public bool GetShouldShow();
    public Sprite GetSprite();
}
