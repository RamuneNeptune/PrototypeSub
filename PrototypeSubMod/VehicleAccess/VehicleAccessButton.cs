using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.VehicleAccess;

public class VehicleAccessButton : MonoBehaviour, ISelectable
{
    [SerializeField] private Button button;
    [SerializeField] private RectTransform rect;
    
    public bool IsValid() => gameObject.activeSelf;
    public RectTransform GetRect() => rect;
    public Graphic GetGraphic() => button.targetGraphic;

    public void OnGamepadSelect()
    {
        button.OnPointerEnter(new PointerEventData(EventSystem.current));
    }
    
    public void OnGamepadDeselect()
    {
        button.OnPointerExit(new PointerEventData(EventSystem.current));
    }
    
    public bool OnButtonDown(GameInput.Button pressedButton)
    {
        if (!GameInput.controllerEnabled) return false;

        if (pressedButton != GameInput.Button.LeftHand) return false;

        button.OnPointerClick(new PointerEventData(EventSystem.current));
        return true;
    }
}