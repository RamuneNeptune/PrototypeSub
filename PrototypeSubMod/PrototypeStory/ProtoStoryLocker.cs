﻿using PrototypeSubMod.EngineLever;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Patches;
using PrototypeSubMod.Teleporter;
using PrototypeSubMod.Upgrades;
using SubLibrary.Monobehaviors;
using System;
using System.Collections;
using System.Linq;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.UI.AbilitySelection;
using PrototypeSubMod.Utility;
using Story;
using SubLibrary.SubFire;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class ProtoStoryLocker : MonoBehaviour
{
    [SaveStateReference(false)]
    public static bool StoryEndingActive;
    [SaveStateReference(false)]
    public static bool WithinSaveLockZone;

    public static event Action onEndingStart;

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
    [SerializeField] private GameObject[] interceptorButtons;
    [SerializeField] private InterfloorTeleporter[] teleporters;
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

        var tetherManager = subRoot.GetComponentInChildren<TetherManager>();
        foreach (var upgrade in upgradeManager.GetInstalledUpgrades())
        {
            upgrade.SetUpgradeEnabled(false);
            upgrade.SetUpgradeLocked(true);
            tetherManager.UpdateIcon(upgrade);
        }

        motorHandler.RemoveAllNoiseOverrides();
        motorHandler.RemoveAllPowerMultipliers();
        motorHandler.RemoveAllSpeedBonuses();
        motorHandler.RemoveAllSpeedMultipliers();
        motorHandler.AddPowerEfficiencyMultiplier(new ProtoMotorHandler.ValueRegistrar(this, 9999));
        hydrolockCloseTrigger.SetActive(true);
        engineLever.SetStoryLocked(true);
        subRoot.GetComponent<Stabilizer>().uprightAccelerationStiffness = 100;
        subRoot.GetComponentInChildren<ProtoFinsManager>().UpdateMotorBonuses();

        foreach (var button in interceptorButtons)
        {
            button.SetActive(false);
        }
        
        foreach (var foldManager in subRoot.GetComponentsInChildren<FinFoldManager>())
        {
            Destroy(foldManager);
        }

        enteredFullLock = true;

        foreach (var teleporter in teleporters)
        {
            var col = teleporter.GetComponent<Collider>();
            if (col) col.enabled = false;
        }
        
        foreach (var room in subRoot.GetComponentsInChildren<SubRoom>(true))
        {
            var nodes = room.GetSpawnNodes();
            foreach (var node in nodes)
            {
                for (int i = 0; i < node.childCount; i++)
                {
                    Destroy(node.GetChild(i).gameObject);
                }
            }
        }

        var pings = PingManager.pings.Values.ToArray();
        foreach (var val in pings)
        {
            if (val.gameObject == subRoot.gameObject) continue;
            val.gameObject.SetActive(false);
        }
        
        onEndingStart?.Invoke();
    }

    private void OnEnterSaveLock()
    {
        Destroy(subRoot.GetComponent<AttackableLikeCyclops>());
        IngameMenu_Patches.SetDenySaving(true);
        StoryGoalManager.main.OnGoalComplete("OnEnterStoryEndProximity");
    }

    private void OnExitSaveLock()
    {
        subRoot.gameObject.EnsureComponent<AttackableLikeCyclops>();
        IngameMenu_Patches.SetDenySaving(false);
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
