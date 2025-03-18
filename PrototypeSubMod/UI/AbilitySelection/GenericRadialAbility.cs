using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.UI.AbilitySelection;

internal class GenericRadialAbility : MonoBehaviour, IAbilityIcon
{
    [SerializeField] private bool showAbility = true;
    [SerializeField] private bool allowActivationWhenActive = true;
    [SerializeField] private Sprite sprite;
    [Tooltip("This object should be set inactive when the upgrade is inactive, and vice versa")]
    [SerializeField] private GameObject upgradeActiveObject;
    [SerializeField] private UnityEvent onActivated;
    [SerializeField] private UnityEvent onUnselected;

    public bool GetShouldShow() => showAbility;
    public Sprite GetSprite() => sprite;

    public void OnActivated()
    {
        if (GetActive() && !allowActivationWhenActive) return;

        onActivated?.Invoke();
    }

    public void OnSelectedChanged(bool changed)
    {
        if (!changed)
        {
            onUnselected?.Invoke();
        }
    }

    public bool GetActive()
    {
        if (!upgradeActiveObject) return false;

        return upgradeActiveObject.activeSelf;
    }
}
