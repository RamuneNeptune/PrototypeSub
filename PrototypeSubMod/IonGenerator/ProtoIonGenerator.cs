using PrototypeSubMod.Interfaces;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.IonGenerator;

internal class ProtoIonGenerator : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private float energyPerSecond = 0.3f;
    [SerializeField] private float activeNoiseValue;
    [SerializeField] private float empChargeUpTime = 300;

    [Header("EMP")]
    [SerializeField] private Transform empSpawnPos;
    [SerializeField] private float empLifetime;
    [SerializeField] private AnimationCurve blastRadius;
    [SerializeField] private AnimationCurve blastHeight;
    [SerializeField] private float disableElectronicsTime;

    private GameObject empPrefab;
    private bool upgradeActive;
    private bool empFired;
    private float[] originalSpeedValues;
    private float currentEMPChargeTime;

    private IEnumerator Start()
    {
        originalSpeedValues = motorMode.motorModeSpeeds;

        CoroutineTask<GameObject> crabsquidTask = CraftData.GetPrefabForTechTypeAsync(TechType.CrabSquid);

        yield return crabsquidTask;

        GameObject crabsquid = crabsquidTask.result.Get();
        var empAttack = crabsquid.GetComponent<EMPAttack>();

        var emp = empAttack.ammoPrefab;
        empPrefab = Instantiate(emp, new Vector3(0, 500, 0), Quaternion.identity);
        var empBlast = empPrefab.GetComponent<EMPBlast>();

        empBlast.lifeTime = empLifetime;
        empBlast.blastRadius = blastRadius;
        empBlast.blastHeight = blastHeight;
        empBlast.disableElectronicsTime = disableElectronicsTime;
    }

    private void Update()
    {
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
        }

        if(currentEMPChargeTime < empChargeUpTime)
        {
            currentEMPChargeTime += Time.deltaTime;
            powerRelay.AddEnergy(energyPerSecond * Time.deltaTime, out _);
        }
        else if(!empFired)
        {
            //Do EMP thing
            Instantiate(empPrefab, empSpawnPos.position, empSpawnPos.rotation);
            empFired = true;
        }

        motorMode.motorModeSpeeds = new float[originalSpeedValues.Length];
    }

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }

    public bool GetUpgradeActive() => upgradeActive;

    public float GetNoiseValue() => activeNoiseValue;
}
