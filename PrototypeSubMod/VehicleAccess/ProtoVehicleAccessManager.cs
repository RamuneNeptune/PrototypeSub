using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.VehicleAccess;

public class ProtoVehicleAccessManager : MonoBehaviour, uGUI_INavigableIconGrid
{
    [SerializeField] private VehicleAccessButton[] accessButtons;

    private RectTransform rectTransform;
    private ProtoVehicleAccessTerminal terminal;
    private VehicleAccessButton selectedButton;
    private VehicleAccessReturnManager returnManager;
    private uGUI_InventoryTab inventoryTab;
    private int buttonIndex;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        returnManager = uGUI_PDA.main.GetComponentInChildren<VehicleAccessReturnManager>(true);
    }

    public void OpenUpgrades()
    {
        terminal.OpenUpgrades(_ => returnManager.gameObject.SetActive(false));
        returnManager.gameObject.SetActive(true);
        inventoryTab.usedStorageGrids.Add(returnManager);
    }

    public void OpenStorage()
    {
        terminal.OpenStorage(_ => returnManager.gameObject.SetActive(false));
        returnManager.gameObject.SetActive(true);
        inventoryTab.usedStorageGrids.Add(returnManager);
    }

    public void ReturnToManager()
    {
        terminal.QuickOpenManager();
    }

    public void SetInventoryTab(uGUI_InventoryTab inventoryTab)
    {
        this.inventoryTab = inventoryTab;
    }

    public void SetTerminal(ProtoVehicleAccessTerminal terminal)
    {
        this.terminal = terminal;
    }

    public object GetSelectedItem() => selectedButton;
    public Graphic GetSelectedIcon() => selectedButton?.GetGraphic();

    public void SelectItem(object item)
    {
        if (item is not VehicleAccessButton button)
        {
            Plugin.Logger.LogError($"Tried to select {item} on {gameObject} but it is not a VehicleAccessButton!");
            return;
        }

        selectedButton?.OnGamepadDeselect();
        selectedButton = button;
        UISelection.selected = button;
        button.OnGamepadSelect();
    }

    public void DeselectItem()
    {
        selectedButton?.OnGamepadDeselect();
        selectedButton = null;
        UISelection.selected = null;
    }

    public bool SelectFirstItem()
    {
        SelectItem(accessButtons[0]);
        GamepadInputModule.current.SetCurrentGrid(this);
        return true;
    }

    public bool SelectItemClosestToPosition(Vector3 worldPos)
    {
        UISelection.sSelectables.Clear();
        UISelection.sSelectables.AddRange(accessButtons);
        var selectable = UISelection.FindSelectable(rectTransform, worldPos, UISelection.sSelectables);
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
        if (dirX == 0 && dirY == 0) return false;

        if (dirX == 0) return false;

        if ((dirX < 0 && selectedButton == accessButtons[0]) ||
            (dirX > 0 && selectedButton == accessButtons[1])) return false;

        buttonIndex = Mathf.Clamp(buttonIndex + dirX, 0, accessButtons.Length - 1);
        SelectItem(accessButtons[buttonIndex]);
        return true;
    }

    public uGUI_INavigableIconGrid GetNavigableGridInDirection(int dirX, int dirY) =>
        inventoryTab.GetNavigableGridInDirection(this, dirX, dirY);

    public bool ShowSelector { get; } = true;
    public bool EmulateRaycast { get; } = true;
}