using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Upgrades;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class ProtoStoryLocker : MonoBehaviour
{
    public static bool StoryEndingActive;
    public static bool WithinSaveLockZone;

    [Header("Activation")]
    [SerializeField] private float activationDistance;
    [SerializeField] private float saveLockDistance;
    [SerializeField] private float checkInterval;

    [Header("Locking")]
    [SerializeField] private LiveMixin mixin;
    [SerializeField] private ProtoUpgradeManager upgradeManager;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private GameObject hydrolockCloseTrigger;
    [SerializeField] private Animator watergateAnimator;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(CheckRepeated());
        hydrolockCloseTrigger.SetActive(false);
    }

    private IEnumerator CheckRepeated()
    {
        while(true)
        {
            if (StoryEndingActive) yield break;
            yield return new WaitForSeconds(checkInterval);

            CheckForDistance();
        }
    }

    private void CheckForDistance()
    {
        float dist = (Plugin.STORY_END_POS - transform.position).sqrMagnitude;
        if (dist < (activationDistance * activationDistance))
        {
            OnEnterStoryEnding();
            CancelInvoke(nameof(CheckForDistance));
        }

        if (dist < (saveLockDistance * saveLockDistance))
        {
            WithinSaveLockZone = true;
        }
    }

    private void OnEnterStoryEnding()
    {
        StoryEndingActive = true;
        mixin.invincible = true;

        ErrorMessage.AddError("Entered story ending; locking upgrades");
        foreach (var upgrade in upgradeManager.GetInstalledUpgrades())
        {
            upgrade.SetUpgradeEnabled(false);
            upgrade.SetUpgradeLocked(true);
        }

        motorHandler.RemoveAllNoiseOverrides();
        motorHandler.RemoveAllPowerMultipliers();
        motorHandler.RemoveAllSpeedBonuses();
        motorHandler.RemoveAllSpeedMultipliers();
        motorHandler.AddPowerEfficiencyMultiplier(new ProtoMotorHandler.ValueRegistrar(this, 9999));
        hydrolockCloseTrigger.SetActive(true);
    }

    private void OnDestroy()
    {
        StoryEndingActive = false;
        WithinSaveLockZone = false;
        motorHandler.RemovePowerEfficiencyMultiplier(this);
    }

    public void CloseHydrolock()
    {
        watergateAnimator.SetBool("HydrolockEnabled", true);
    }
}
