using PrototypeSubMod.EngineLever;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Patches;
using PrototypeSubMod.Teleporter;
using PrototypeSubMod.Upgrades;
using SubLibrary.Monobehaviors;
using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class ProtoStoryLocker : MonoBehaviour
{
    public static bool StoryEndingActive;
    public static bool WithinSaveLockZone;

    public event Action onEndingStart;

    [Header("Activation")]
    [SerializeField] private float activationDistance;
    [SerializeField] private float saveLockDistance;
    [SerializeField] private float checkInterval;

    [Header("Locking")]
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private ProtoUpgradeManager upgradeManager;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private GameObject hydrolockCloseTrigger;
    [SerializeField] private Animator watergateAnimator;
    [SerializeField] private ProtoEngineLever engineLever;
    [SerializeField] private ProtoTeleporterTerminalLocker terminalTrigger;
    [SerializeField] private ProtoTeleporterManager teleporterManager;

    private bool wasInLockZone;
    private bool enteredFullLock;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(CheckRepeated());
        hydrolockCloseTrigger.SetActive(false);
    }

    private IEnumerator CheckRepeated()
    {
        while (gameObject != null)
        {
            if (StoryEndingActive) yield break;
            yield return new WaitForSeconds(checkInterval);

            CheckForDistance();
        }
    }

    private void CheckForDistance()
    {
        float dist = (Plugin.STORY_END_POS - transform.position).sqrMagnitude;
        if (dist < (activationDistance * activationDistance) && !enteredFullLock)
        {
            OnEnterStoryEnding();
            CancelInvoke(nameof(CheckForDistance));
        }

        WithinSaveLockZone = dist < (saveLockDistance * saveLockDistance) && Player.main.currentSub == subRoot;
        if (wasInLockZone != WithinSaveLockZone && WithinSaveLockZone)
        {
            OnEnterSaveLock();
        }
        else if (wasInLockZone != WithinSaveLockZone && !WithinSaveLockZone)
        {
            OnExitSaveLock();
        }

        wasInLockZone = WithinSaveLockZone;
    }

    private void OnEnterStoryEnding()
    {
        StoryEndingActive = true;
        subRoot.live.invincible = true;

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
        engineLever.SetStoryLocked(true);

        terminalTrigger.SetStoryLocked(true);
        teleporterManager.ToggleDoor(false);

        enteredFullLock = true;
        IngameMenu_Patches.SetDenySaving(true);

        onEndingStart?.Invoke();
    }

    private void OnEnterSaveLock()
    {
        Destroy(subRoot.GetComponent<AttackableLikeCyclops>());
    }

    private void OnExitSaveLock()
    {
        subRoot.gameObject.EnsureComponent<AttackableLikeCyclops>();
    }

    private void OnDestroy()
    {
        StoryEndingActive = false;
        WithinSaveLockZone = false;
        motorHandler.RemovePowerEfficiencyMultiplier(this);
        IngameMenu_Patches.SetDenySaving(false);
    }

    public void CloseHydrolock()
    {
        watergateAnimator.SetBool("HydrolockEnabled", true);
    }
}
