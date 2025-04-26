using System;
using SubLibrary.UI;
using UnityEngine;

namespace PrototypeSubMod.UI.NoiseDisplay;

public class ProtoNoiseDisplay : MonoBehaviour, IUIElement
{
    [SerializeField] private CyclopsNoiseManager noiseManager;
    [SerializeField] private Transform[] noiseNotches;
    
    public void UpdateUI()
    {
        if (Time.deltaTime == 0) return;
        
        for (int i = 0; i < noiseNotches.Length; i++)
        {
            bool active = i <= (noiseManager.GetNoisePercent() * 4 - 1);
            noiseNotches[i].gameObject.SetActive(active);
        }
    }

    public void OnSubDestroyed()
    {
        foreach (var notch in noiseNotches)
        {
            notch.gameObject.SetActive(false);
        }
    }
}