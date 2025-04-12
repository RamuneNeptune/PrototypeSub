using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.PowerSystem;

public class RelayUninstallationButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float confirmTime = 2f;
    
    private ProtoPowerRelay powerRelay;
    private bool hovered;
    private bool consumed;
    private bool wasClicking;

    private float currentConfirmTime;

    private void OnValidate()
    {
        if (!image) TryGetComponent(out image);
    }

    private void Start()
    {
        powerRelay = GetComponentInParent<ProtoPowerRelay>();
    }
    
    private void Update()
    {
        bool clicking = GameInput.GetButtonHeld(GameInput.Button.LeftHand);

        if (clicking && !wasClicking)
        {
            consumed = false;
        }
        
        if (!hovered || !clicking || consumed)
        {
            image.fillAmount = 1f;
            currentConfirmTime = 0;
            wasClicking = clicking;
            return;
        }

        if (currentConfirmTime < confirmTime)
        {
            currentConfirmTime += Time.deltaTime;
            image.fillAmount = 1 - (currentConfirmTime / confirmTime);
        }
        else
        {
            powerRelay.UninstallSource();
            currentConfirmTime = 0;
            consumed = true;
        }
        
        wasClicking = clicking;
    }

    public void OnPointerEnter(BaseEventData data)
    {
        hovered = true;
    }

    public void OnPointerExit(BaseEventData data)
    {
        hovered = false;
    }
}