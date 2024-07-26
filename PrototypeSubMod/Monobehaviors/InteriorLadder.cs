using System;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class InteriorLadder : HandTarget, IHandTarget
{
    [SerializeField] private bool climbsUp;
    [SerializeField] private Transform exitPosition;

    public void OnHandHover(GUIHand hand)
    {
        string text = climbsUp ? "ClimbUp" : "ClimbDown";
        HandReticle.main.SetText(HandReticle.TextType.Hand, text, true, GameInput.Button.LeftHand);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnHandClick(GUIHand hand)
    {
        hand.player.SetPosition(exitPosition.position);
    }
}
