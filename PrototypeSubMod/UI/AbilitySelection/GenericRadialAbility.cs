using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.UI.AbilitySelection;

internal class GenericRadialAbility : MonoBehaviour, IAbilityIcon
{
    [SerializeField] private bool showAbility = true;
    [SerializeField] private Sprite sprite;
    [SerializeField] private UnityEvent onActivated;

    public bool GetShouldShow() => showAbility;
    public Sprite GetSprite() => sprite;

    public void OnActivated()
    {
        onActivated?.Invoke();
    }

    public void OnSelectedChanged(bool changed) { }
}
