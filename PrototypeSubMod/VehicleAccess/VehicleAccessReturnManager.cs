using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.VehicleAccess;

public class VehicleAccessReturnManager : MonoBehaviour, uGUI_INavigableIconGrid
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private VehicleAccessButton button;

    private object selectedItem;
    private ProtoVehicleAccessManager accessManager;
    private uGUI_InventoryTab inventoryTab;
    
    private void Start()
    {
        accessManager = uGUI_PDA.main.GetComponentInChildren<ProtoVehicleAccessManager>(true);
        inventoryTab = uGUI_PDA.main.GetComponentInChildren<uGUI_InventoryTab>(true);
    }

    public void ReturnToManager()
    {
        accessManager.ReturnToManager();
        gameObject.SetActive(false);
    }

    public object GetSelectedItem() => selectedItem;
    public Graphic GetSelectedIcon() => selectedItem != null ? button?.GetGraphic() : null;

    public void SelectItem(object item)
    {
        if (item is VehicleAccessButton b && b == button)
        {
            if (selectedItem == null)
            {
                button.OnGamepadSelect();
            }
        }
        selectedItem = item;
    }

    public void DeselectItem()
    {
        button.OnGamepadDeselect();
        SelectItem(null);
    }

    public bool SelectFirstItem()
    {
        SelectItem(button);
        return true;
    }

    public bool SelectItemClosestToPosition(Vector3 worldPos)
    {
        UISelection.sSelectables.Clear();
        UISelection.sSelectables.Add(button);
        var selectable = UISelection.FindSelectable(rect, worldPos, UISelection.sSelectables);
        UISelection.sSelectables.Clear();
        if (selectable != null)
        {
            SelectItem(selectable);
            return true;
        }

        return false;
    }

    public bool SelectItemInDirection(int dirX, int dirY)
    {
        if (dirX == 0 && dirY == 0) return true;

        return false;
    }

    public uGUI_INavigableIconGrid GetNavigableGridInDirection(int dirX, int dirY) =>
        inventoryTab.GetNavigableGridInDirection(this, dirX, dirY);

    public RectTransform GetRect() => rect;

    public bool ShowSelector { get; } = true;
    public bool EmulateRaycast { get; } = true;
}