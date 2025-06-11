using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.UI.AbilitySelection;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.Docking;

internal class DockingRadialAbility : ProtoUpgrade
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private SelectionMenuManager selectionMenuManager;
    [SerializeField] private ProtoFinsManager finsManager;
    [SerializeField] private ProtoDockingManager dockingManager;

    private void Start()
    {
        finsManager.onFinCountChanged += () =>
        {
            if (finsManager.GetInstalledFinCount() < 2) return;

            selectionMenuManager.RefreshIcons();
        };

        dockingManager.onDockedStatusChanged += selectionMenuManager.RefreshIcons;
    }
    
    public override bool OnActivated()
    {
        dockingManager.Undock();
        return true;
    }

    public override void OnSelectedChanged(bool changed) { }
    public override bool GetShouldShow()
    {
        return finsManager.GetInstalledFinCount() >= 2 && dockingManager.GetDockingBay().dockedVehicle;
    }

    public override TechType GetTechType() => TechType.None;
    public override Sprite GetSprite() => sprite;
}