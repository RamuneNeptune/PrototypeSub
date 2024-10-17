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

    public float GetDefaultTime() => defaultUpgradeTime;
    public float GetUpgradedTime() => variableStreamsUpgradeTime;

    /// <summary>
    /// Returns either the normal or upgraded duration depending on if the upgrade is installed
    /// </summary>
    /// <returns></returns>
    public float GetApplicableDuration()
    {
        return upgradeInstalled ? variableStreamsUpgradeTime : defaultUpgradeTime;
    }

    public void OnLateSaveDataLoaded(BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();
        if (protoData.installedPowerUpgradeType == null) return;

        var component = functionalityRoot.gameObject.AddComponent(protoData.installedPowerUpgradeType);
        (component as PowerSourceFunctionality).SetTime(protoData.currentPowerEffectDuration);
        //(component as PowerSourceFunctionality).OnAbilityActivated();
    }
}
