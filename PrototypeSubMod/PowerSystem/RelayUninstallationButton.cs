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
        bool inRange = (Player.main.transform.position - image.transform.position).sqrMagnitude < 4f;
        bool clicking = GameInput.GetButtonHeld(GameInput.Button.LeftHand);
        UpdateHandText(inRange);
        
        if (clicking && !wasClicking)
        {
            consumed = false;
        }
        
        if (!hovered || !clicking || consumed || !inRange)
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

    private void UpdateHandText(bool inRange)
    {
        if (!inRange || !hovered || consumed) return;
        
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "ProtoUninstallSource", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
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