using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Upgrades;
using System.Collections;
using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class ProtoIonGenerator : ProtoUpgrade
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private VoiceNotification overheatNotification;
    [SerializeField] private float secondsToFillCharge;
    [SerializeField] private float activeNoiseValue;
    [SerializeField] private float empChargeUpTime = 300;
    [SerializeField] private float overheatVoicelineThreshold;
    [SerializeField] private float empOxygenDisableTime = 150f;

    [Header("EMP")] [SerializeField] private EmpSpawner empSpawner;
    [SerializeField] private VoiceNotification empNotification;
    [SerializeField] private FMOD_CustomEmitter empSoundEffect;
    [SerializeField] private float soundEffectVolume = 20f;
    [SerializeField] private float disableElectronicsTime;

    private SubControl subControl;
    private bool empFired;
    private float currentEMPChargeTime;
    private float energyMultiplier = 1;
    private float chargePerSec;
    private bool couldAccel;

    private void Start()
    {
        chargePerSec = PrototypePowerSystem.CHARGE_POWER_AMOUNT / secondsToFillCharge;
        subControl = subRoot.GetComponent<SubControl>();
    }

    private void Update()
    {
        if (!upgradeInstalled)
        {
            motorHandler.SetAllowedToMove(true);
            return;
        }

        motorHandler.SetAllowedToMove(!upgradeEnabled);
        if (upgradeEnabled)
        {
            motorHandler.AddOverrideNoiseValue(new ProtoMotorHandler.ValueRegistrar(this, activeNoiseValue));
        }
        else
        {
            motorHandler.RemoveOverrideNoiseValue(this);
        }

        if (!upgradeEnabled)
        {
            if (currentEMPChargeTime > 0)
            {
                currentEMPChargeTime -= Time.deltaTime;
            }
            else
            {
                empFired = false;
            }

            return;
        }

        if (currentEMPChargeTime >= overheatVoicelineThreshold && !empFired)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(overheatNotification, false, true);
        }

        if (currentEMPChargeTime < empChargeUpTime && !empFired)
        {
            currentEMPChargeTime += Time.deltaTime;
            subRoot.powerRelay.AddEnergy(chargePerSec * Time.deltaTime * energyMultiplier, out _);
        }
        else if (!empFired)
        {
            StartCoroutine(FireEMP());
            empFired = true;
        }
    }

    private IEnumerator FireEMP()
    {
        subRoot.voiceNotificationManager.PlayVoiceNotification(empNotification, false, true);
        yield return new WaitForSeconds(9.7f);

        //Do EMP thing
        upgradeEnabled = false;
        empSpawner.FireEMP(disableElectronicsTime);

        subRoot.powerRelay.DisableElectronicsForTime(empOxygenDisableTime);
        Utils.PlayEnvSound(empSoundEffect, empSpawner.GetSpawnPos().position, soundEffectVolume);
    }

    public void SetEnergyMultiplier(float multiplier)
    {
        energyMultiplier = multiplier;
    }

    public override void OnActivated()
    {
        SetUpgradeEnabled(!upgradeEnabled);
    }

    public override void OnSelectedChanged(bool changed)
    {
        if (!changed)
        {
            SetUpgradeEnabled(false);
        }
    }
}