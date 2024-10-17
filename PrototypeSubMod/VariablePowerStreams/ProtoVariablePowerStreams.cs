using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.VariablePowerStreams;

internal class ProtoVariablePowerStreams : ProtoUpgrade
{
    [SerializeField] private float defaultUpgradeTime = 600f;
    [SerializeField] private float variableStreamsUpgradeTime = 1800f;

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
}
