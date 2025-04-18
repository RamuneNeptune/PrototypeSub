using System;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.PowerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.PowerDisplay;

public class ChargeDeltaDisplay : MonoBehaviour
{
    [SerializeField] private OnModifyPowerEvent onModifyPower;
    [SerializeField] private Image[] masks;
    [SerializeField] private float smoothSpeed;

    private float frameDraw;
    private float currentFill;
    
    private void Start()
    {
        onModifyPower.onModifyPower += OnModifyPower;
        SetFillValues(0);
    }

    private void LateUpdate()
    {
        frameDraw = Mathf.Clamp01(frameDraw);
        currentFill = Mathf.Lerp(currentFill, frameDraw, smoothSpeed * Time.deltaTime);
        SetFillValues(currentFill);

        frameDraw = 0;
    }

    private void OnModifyPower(float power)
    {
        if (power >= 0)
        {
            return;
        }

        if (-power > PrototypePowerSystem.CHARGE_POWER_AMOUNT) return;
        
        frameDraw += -power;
    }

    private void SetFillValues(float value)
    {
        foreach (var mask in masks)
        {
            mask.fillAmount = value;
        }
    }
}