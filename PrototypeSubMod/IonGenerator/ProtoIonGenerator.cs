using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Upgrades;
using SubLibrary.SubFire;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class ProtoIonGenerator : ProtoUpgrade
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private VoiceNotification overheatNotification;
    [SerializeField] private float energyPerSecond = 0.3f;
    [SerializeField] private float activeNoiseValue;
    [SerializeField] private float empChargeUpTime = 300;
    [SerializeField] private float overheatVoicelineThreshold;
    [SerializeField] private float empOxygenDisableTime = 150f;

    [Header("EMP")]
    [SerializeField] private VoiceNotification empNotification;
    [SerializeField] private Transform empSpawnPos;
    [SerializeField] private ModdedSubFire subFire;
    [SerializeField] private SubRoom engineRoom;
    [SerializeField] private float empLifetime;
    [SerializeField] private AnimationCurve blastRadius;
    [SerializeField] private AnimationCurve blastHeight;
    [SerializeField] private float disableElectronicsTime;
    [SerializeField] private FMOD_CustomEmitter empSoundEffect;
    [SerializeField] private float soundEffectVolume = 20f;

    private GameObject empPrefab;
    private bool empFired;
    private float currentEMPChargeTime;
    private float energyMultiplier = 1;

    private IEnumerator Start()
    {
        CoroutineTask<GameObject> crabsquidTask = CraftData.GetPrefabForTechTypeAsync(TechType.CrabSquid);

        yield return crabsquidTask;

        GameObject crabsquid = crabsquidTask.result.Get();
        var empAttack = crabsquid.GetComponent<EMPAttack>();

        empAttack.ammoPrefab.SetActive(false);
        empPrefab = Instantiate(empAttack.ammoPrefab);

        empAttack.ammoPrefab.SetActive(true);
        var empBlast = empPrefab.GetComponent<EMPBlast>();

        empBlast.lifeTime = empLifetime;
        empBlast.blastRadius = blastRadius;
        empBlast.blastHeight = blastHeight;
        empBlast.disableElectronicsTime = disableElectronicsTime;
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
            subRoot.voiceNotificationManager.PlayVoiceNotification(overheatNotification);
        }

        if (currentEMPChargeTime < empChargeUpTime && !empFired)
        {
            currentEMPChargeTime += Time.deltaTime;
            subRoot.powerRelay.AddEnergy(energyPerSecond * Time.deltaTime * energyMultiplier, out _);
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
        yield return new WaitForSeconds(10.5f);

        //Do EMP thing
        var newEMP = Instantiate(empPrefab, empSpawnPos.position, empSpawnPos.rotation, empSpawnPos);
        newEMP.SetActive(true);
        upgradeEnabled = false;

        subRoot.powerRelay.DisableElectronicsForTime(empOxygenDisableTime);
        Utils.PlayEnvSound(empSoundEffect, empSpawnPos.position, soundEffectVolume);
        subFire.CreateFire(engineRoom);
    }

    public void SetEnergyMultiplier(float multiplier)
    {
        energyMultiplier = multiplier;
    }
}
