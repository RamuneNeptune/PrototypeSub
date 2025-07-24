using System;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.ProtoStrafe;

internal class ProtoStrafe : ProtoUpgrade
{
    [SerializeField] private float sidewaysAccel;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private SubControl subControl;

    /*
    private void Update()
    {
        if (GameInput.GetButtonDown(GameInput.Button.Deconstruct))
        {
            SetUpgradeEnabled(!upgradeEnabled);
        }
    }
    */

    private void FixedUpdate()
    {
        if (!upgradeEnabled) return;

        rigidbody.AddForce(transform.right * (sidewaysAccel * subControl.throttle.x), ForceMode.Acceleration);
    }
    
    public override bool OnActivated()
    {
        SetUpgradeEnabled(!upgradeEnabled);
        return true;
    }

    public override void OnSelectedChanged(bool changed) { }
}