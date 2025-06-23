using Nautilus.Handlers;
using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.UI.AbilitySelection;
using PrototypeSubMod.Upgrades;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PrototypeSubMod.Overclock;

internal class ProtoOverclockModule : ProtoUpgrade
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsExternalDamageManager damageManager;
    [SerializeField] private FMOD_CustomLoopingEmitter loopingEmitter;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private VoiceNotification enabledVoiceline;
    [SerializeField] private VoiceNotification invalidOperationNotification;
    [SerializeField] private float fovIncrease;
    [SerializeField] private float speedBaseBonus;
    [SerializeField] private float turningTorqueMultiplier;
    [SerializeField] private float secondsToDrainCharge;
    [SerializeField, Range(0, 1)] private float chanceForHullBreach;
    [SerializeField] private float hullBreachMinActiveTime;
    [SerializeField] private float minTimeBetweenBreaches;
    [SerializeField] private float sfxRampUpTime = 2f;

    private PilotingChair chair;
    private PDACameraFOVControl pdaCameraControl;
    private float currentHullBreachTime;
    private float currentTimeBetweenBreaches;
    private float currentRampUpTime;

    private void Start()
    {
        chair = subRoot.GetComponentInChildren<PilotingChair>();
        pdaCameraControl = Player.main.GetComponent<PDACameraFOVControl>();
    }
    
    private void Update()
    {
        if (!upgradeInstalled || ionGenerator.GetUpgradeEnabled())
        {
            motorHandler.RemoveSpeedBonus(this);
            motorHandler.RemoveTurningTorqueMultiplier(this);
            return;
        }

        if (Player.main.currChair != chair && upgradeEnabled)
        {
            SetUpgradeEnabled(false);
            subRoot.GetComponentInChildren<TetherManager>().ForceSelectedIconUpdate();
            return;
        }

        float normalizedSpeed = motorHandler.GetNormalizedSpeed();
        bool couldConsume = false;
        if (upgradeEnabled)
        {
            couldConsume = subRoot.powerRelay.ConsumeEnergy(
                PrototypePowerSystem.CHARGE_POWER_AMOUNT / secondsToDrainCharge * Time.deltaTime * Mathf.Clamp(normalizedSpeed, 0.2f, 1), out _);
        }
        
        HandleHullBreaches(couldConsume, normalizedSpeed);

        if (!upgradeEnabled)
        {
            motorHandler.RemoveSpeedBonus(this);
            motorHandler.RemoveTurningTorqueMultiplier(this);
            return;
        }
        
        motorHandler.AddSpeedBonus(new ProtoMotorHandler.ValueRegistrar(this, speedBaseBonus));
        motorHandler.AddTurningTorqueMultiplier(new  ProtoMotorHandler.ValueRegistrar(this, turningTorqueMultiplier));
        
        MainCameraControl.main.ShakeCamera(0.2f * normalizedSpeed);
        SNCameraRoot.main.SetFov(Mathf.Lerp(SNCameraRoot.main.CurrentFieldOfView,
            MiscSettings.fieldOfView + fovIncrease * normalizedSpeed, Time.deltaTime * 2f));

        HandleSFXVolume();
    }

    private void HandleSFXVolume()
    {
        if (currentRampUpTime < sfxRampUpTime)
        {
            currentRampUpTime += Time.deltaTime;
        }
        
        if (!CustomSoundHandler.TryGetCustomSoundChannel(loopingEmitter.GetInstanceID(), out var loopingChannel))
            return;

        float rampUpMultiplier = currentRampUpTime / sfxRampUpTime;
        loopingChannel.setVolume(motorHandler.GetNormalizedSpeed() * rampUpMultiplier);
    }

    private void HandleHullBreaches(bool couldConsume, float normalizedSpeed)
    {
        if (GetUpgradeEnabled() && currentHullBreachTime < hullBreachMinActiveTime)
        {
            currentHullBreachTime += Time.deltaTime * normalizedSpeed;
        }
        else if (!GetUpgradeEnabled() && currentHullBreachTime > 0)
        {
            currentHullBreachTime -= Time.deltaTime;
        }

        if (GameModeUtils.IsInvisible()) return;

        if (currentHullBreachTime < hullBreachMinActiveTime)
        {
            return;
        }

        if (damageManager.unusedDamagePoints.Count == 0) return;

        if (currentTimeBetweenBreaches <= 0 && couldConsume)
        {
            if (Random.Range(0f, 1f) < chanceForHullBreach)
            {
                damageManager.CreatePoint();
            }

            currentTimeBetweenBreaches = minTimeBetweenBreaches;
        }
        else if (couldConsume)
        {
            currentTimeBetweenBreaches -= Time.deltaTime;
        }
    }

    public override bool GetUpgradeEnabled()
    {
        return upgradeEnabled && !ionGenerator.GetUpgradeEnabled();
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        base.SetUpgradeEnabled(enabled);

        pdaCameraControl.enabled = !enabled;
        if (upgradeEnabled)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(enabledVoiceline);
            loopingEmitter.Play();
            currentRampUpTime = 0;
        }
        else
        {
            loopingEmitter.Stop();
        }
    }

    public override bool OnActivated()
    {
        if (ionGenerator.GetUpgradeEnabled())
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(invalidOperationNotification);
            return false;
        }
        
        SetUpgradeEnabled(!upgradeEnabled);
        return true;
    }

    public override void OnSelectedChanged(bool changed) { }
}
