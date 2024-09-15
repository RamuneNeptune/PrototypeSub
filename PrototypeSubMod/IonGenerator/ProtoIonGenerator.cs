using PrototypeSubMod.Interfaces;
using PrototypeSubMod.MotorHandler;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class ProtoIonGenerator : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private float energyPerSecond = 0.3f;
    [SerializeField] private float activeNoiseValue;
    [SerializeField] private float empChargeUpTime = 300;
    [SerializeField] private float empOxygenDisableTime = 150f;

    [Header("EMP")]
    [SerializeField] private Transform empSpawnPos;
    [SerializeField] private float empLifetime;
    [SerializeField] private AnimationCurve blastRadius;
    [SerializeField] private AnimationCurve blastHeight;
    [SerializeField] private float disableElectronicsTime;
    [SerializeField] private FMOD_CustomEmitter empSoundEffect;
    [SerializeField] private float soundEffectVolume = 20f;

    private GameObject empPrefab;
    private bool upgradeActive;
    private bool upgradeInstalled;
    private bool empFired;
    private float currentEMPChargeTime;

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

        motorHandler.SetAllowedToMove(!upgradeActive);

        if (!upgradeActive)
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

        if (currentEMPChargeTime < empChargeUpTime && !empFired)
        {
            currentEMPChargeTime += Time.deltaTime;
            powerRelay.AddEnergy(energyPerSecond * Time.deltaTime, out _);
        }
        else if (!empFired)
        {
            //Do EMP thing
            var newEMP = Instantiate(empPrefab, empSpawnPos.position, empSpawnPos.rotation, empSpawnPos);
            newEMP.SetActive(true);
            empFired = true;
            upgradeActive = false;

            powerRelay.DisableElectronicsForTime(empOxygenDisableTime);

            Utils.PlayEnvSound(empSoundEffect, empSpawnPos.position, soundEffectVolume);
        }
    }

    public void SetUpgradeInstalled(bool installed)
    {
        upgradeInstalled = installed;
    }

    public bool GetUpgradeInstalled() => upgradeInstalled;

    public float GetNoiseValue() => activeNoiseValue;

    public string GetUpgradeName()
    {
        return "Ion Generator";
    }

    public void SetUpgradeEnabled(bool enabled)
    {
        upgradeActive = enabled;
    }

    public bool GetUpgradeEnabled() => upgradeActive;
}
