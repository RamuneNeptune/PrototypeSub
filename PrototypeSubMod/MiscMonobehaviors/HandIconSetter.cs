using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class HandIconSetter : MonoBehaviour
{
    private bool mouseHovered;
    
    public void MouseEnter()
    {
        mouseHovered = true;
    }

    public void MouseExit()
    {
        mouseHovered = false;
    }

    private void Update()
    {
        if (!mouseHovered) return;
        
        HandReticle.main.SetIcon(HandReticle.IconType.Hand);
        HandReticle.main.SetText(HandReticle.TextType.Hand, string.Empty, false);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false);
    }
}