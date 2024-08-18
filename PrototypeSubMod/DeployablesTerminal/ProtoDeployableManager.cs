using PrototypeSubMod.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class ProtoDeployableManager : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private VoiceNotification launchLightNotification;
    [SerializeField] private VoiceNotification launchDecoyNotification;
    [SerializeField] private GameObject lightPrefab;
    [SerializeField] DeployablesStorageTerminal storageTerminal;
    [SerializeField] private Transform lightSpawnTransform;
    [SerializeField] private Transform decoySpawnTransform;
    [SerializeField] private float launchLightDelay;
    [SerializeField] private float lightLaunchForce;
    [SerializeField] private float launchDecoyDelay;

    private bool upgradeActive;
    private GameObject decoyPrefab;

    private IEnumerator Start()
    {
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.CyclopsDecoy);
        yield return task;

        decoyPrefab = task.result.Get();
    }

    public void TryLaunchLight()
    {
        if (!upgradeActive) return;

        int lightCount = 0;
        List<string> filledSlots = new();

        foreach (var slot in DeployablesStorageTerminal.LightBeaconSlots)
        {
            var item = storageTerminal.equipment.GetItemInSlot(slot);

            if (item != null)
            {
                filledSlots.Add(slot);
                lightCount++;
            }
        }

        if(lightCount > 0)
        {
            string slot = filledSlots[filledSlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, false, false);

            Invoke(nameof(SpawnLightDelayed), launchLightDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchLightNotification);
        }
    }

    public void TryLaunchDecoy()
    {
        if (!upgradeActive) return;

        int decoyCount = 0;
        List<string> filledSlots = new();

        foreach (var slot in DeployablesStorageTerminal.CreatureDecoySlots)
        {
            var item = storageTerminal.equipment.GetItemInSlot(slot);

            if (item != null)
            {
                filledSlots.Add(slot);
                decoyCount++;
            }
        }

        if (decoyCount > 0)
        {
            string slot = filledSlots[filledSlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, false, false);

            Invoke(nameof(SpawnDecoyDelayed), launchDecoyDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchDecoyNotification);
        }
    }

    private void SpawnLightDelayed()
    {
        var lightComponent = Instantiate(lightPrefab, lightSpawnTransform.position, lightSpawnTransform.rotation).GetComponent<DeployableLight>();

        lightComponent.LaunchWithForce(lightLaunchForce);
    }

    private void SpawnDecoyDelayed()
    {
        var decoyComponent = Instantiate(decoyPrefab, decoySpawnTransform.position, Quaternion.identity).GetComponent<CyclopsDecoy>();

        if(decoyComponent)
        {
            decoyComponent.launch = true;
        }
    }

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }
}
