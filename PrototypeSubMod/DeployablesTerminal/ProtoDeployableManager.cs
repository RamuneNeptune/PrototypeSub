using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.Upgrades;
using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class ProtoDeployableManager : ProtoUpgrade
{
    [SerializeField] DeployablesStorageTerminal storageTerminal;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private VoiceNotification launchLightNotification;
    [SerializeField] private VoiceNotification launchDecoyNotification;
    [SerializeField] private VoiceNotification invalidOperationNotification;
    [SerializeField] private GameObject lightPrefab;
    [SerializeField] private Transform lightSpawnTransform;
    [SerializeField] private Transform decoySpawnTransform;

    [SerializeField] private int decoyChargeConsumption;
    [SerializeField] private float launchLightDelay;
    [SerializeField] private float lightLaunchForce;
    [SerializeField] private float launchDecoyDelay;

    private int lightCount;
    private int decoyCount;
    private GameObject decoyPrefab;
    private List<string> availableLightSlots = new();
    private List<string> availableDecoySlots = new();

    private IEnumerator Start()
    {
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.CyclopsDecoy);
        yield return task;

        decoyPrefab = task.result.Get();
    }

    public void TryLaunchLight()
    {
        if (ionGenerator.GetUpgradeEnabled() && ionGenerator.GetUpgradeInstalled()) return;

        if (lightCount > 0)
        {
            string slot = availableLightSlots[availableLightSlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, true, false);

            Invoke(nameof(SpawnLightDelayed), launchLightDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchLightNotification);
        }
        else
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(invalidOperationNotification);
        }
    }

    public void TryLaunchDecoy()
    {
        if (ionGenerator.GetUpgradeEnabled() && ionGenerator.GetUpgradeInstalled()) return;

        if (decoyCount > 0)
        {
            subRoot.powerRelay.ConsumeEnergy(PrototypePowerSystem.CHARGE_POWER_AMOUNT * decoyChargeConsumption, out _);
            
            string slot = availableDecoySlots[availableDecoySlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, true, false);

            Invoke(nameof(SpawnDecoyDelayed), launchDecoyDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchDecoyNotification);
        }
        else
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(invalidOperationNotification);
        }
    }

    private void SpawnLightDelayed()
    {
        var lightComponent = Instantiate(lightPrefab, lightSpawnTransform.position, lightSpawnTransform.rotation).GetComponent<DeployableLight>();
        lightComponent.gameObject.SetActive(true);

        lightComponent.LaunchWithForce(lightLaunchForce, subRoot.rb.velocity);
    }

    private void SpawnDecoyDelayed()
    {
        var decoyComponent = Instantiate(decoyPrefab, decoySpawnTransform.position, Quaternion.identity).GetComponent<CyclopsDecoy>();
        decoyComponent.gameObject.SetActive(true);

        if (decoyComponent)
        {
            decoyComponent.launch = true;
        }
    }

    public void RecalculateDeployableTotals()
    {
        lightCount = 0;
        decoyCount = 0;

        foreach (var slot in DeployablesStorageTerminal.LightBeaconSlots)
        {
            var item = storageTerminal.equipment.GetItemInSlot(slot);

            if (item != null)
            {
                availableLightSlots.Add(slot);
                lightCount++;
            }
        }

        foreach (var slot in DeployablesStorageTerminal.CreatureDecoySlots)
        {
            var item = storageTerminal.equipment.GetItemInSlot(slot);

            if (item != null)
            {
                availableDecoySlots.Add(slot);
                decoyCount++;
            }
        }
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;

    // The deployable manager will be called via different icons
    public override void OnActivated() { }
    public override void OnSelectedChanged(bool changed) { }
}
