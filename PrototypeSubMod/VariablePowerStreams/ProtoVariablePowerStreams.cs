using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.SaveData;
using PrototypeSubMod.Upgrades;
using SubLibrary.SaveData;
using UnityEngine;

namespace PrototypeSubMod.VariablePowerStreams;

internal class ProtoVariablePowerStreams : ProtoUpgrade, ILateSaveDataListener
{
    [SerializeField] private float defaultUpgradeTime = 600f;
    [SerializeField] private float variableStreamsUpgradeTime = 1800f;
    [SerializeField] private ChildObjectIdentifier functionalityRoot;

    private bool selected;

    public float GetDefaultTime() => defaultUpgradeTime;
    public float GetUpgradedTime() => variableStreamsUpgradeTime;

    /// <summary>
    /// Returns either the normal or upgraded duration depending on if the upgrade is installed
    /// </summary>
    /// <returns></returns>
    public float GetApplicableDuration()
    {
        return selected ? variableStreamsUpgradeTime : defaultUpgradeTime;
    }

    public void OnLateSaveDataLoaded(BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();
        if (protoData.installedPowerUpgradeType == null) return;

        var component = functionalityRoot.gameObject.AddComponent(protoData.installedPowerUpgradeType);
        (component as PowerSourceFunctionality).SetTime(protoData.currentPowerEffectDuration);

        var abilitySystem = transform.parent.GetComponentInChildren<ProtoPowerAbilitySystem>(true);
        abilitySystem.CheckForCurrentFunctionality();
    }

    public override void OnActivated() { }

    public override void OnSelectedChanged(bool changed)
    {
        selected = changed;
    }
}
