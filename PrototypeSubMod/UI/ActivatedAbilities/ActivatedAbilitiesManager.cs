using PrototypeSubMod.UI.AbilitySelection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.UI.ActivatedAbilities;

internal class ActivatedAbilitiesManager : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform[] abilityIconSlots;
    [SerializeField] private SelectionMenuManager menuManager;
    [SerializeField] private TetherManager tetherManager;

    private List<ActiveAbilityIcon> activeAbilityIcons = new();
    private List<GameObject> actuallyActiveIcons = new();

    private void Start()
    {
        tetherManager.onAbilityActivatedChanged += OnAbilitySelectedChanged;
    }

    public void OnAbilitySelectedChanged(IAbilityIcon icon)
    {
        if (icon == null) return;

        var activeIcon = activeAbilityIcons.FirstOrDefault(i => i != null && i.GetIcon() == icon);
        bool isActive = activeIcon != null && activeIcon.GetIcon().GetActive();

        if (activeIcon == null)
        {
            var newIcon = CreateNewIcon(icon);
            activeAbilityIcons.Add(newIcon);
            activeIcon = newIcon;
            isActive = icon.GetActive();
        }

        if (!activeIcon.gameObject.activeSelf && isActive)
        {
            activeIcon.gameObject.SetActive(true);
            activeIcon.transform.SetParent(abilityIconSlots[GetActiveAbilityCount()]);
            activeIcon.transform.localPosition = Vector3.zero;
            actuallyActiveIcons.Add(activeIcon.gameObject);
        }
        else if (activeIcon && !isActive)
        {
            activeIcon.gameObject.SetActive(false);
            actuallyActiveIcons.Remove(activeIcon.gameObject);
        }
    }
    
    public int GetActiveAbilityCount() => actuallyActiveIcons.Count;

    private ActiveAbilityIcon CreateNewIcon(IAbilityIcon icon)
    {
        var instance = Instantiate(iconPrefab, abilityIconSlots[GetActiveAbilityCount()]);
        instance.transform.localPosition = Vector3.zero;
        var newIcon = instance.GetComponent<ActiveAbilityIcon>();
        newIcon.SetIcon(icon);

        return newIcon;
    }
}
