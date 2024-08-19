using PrototypeSubMod.Interfaces;
using PrototypeSubMod.Monobehaviors;
using SubLibrary.Handlers;
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
        if (!upgradeActive) return;

        if(lightCount > 0)
        {
            string slot = availableLightSlots[availableLightSlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, true, false);

            Invoke(nameof(SpawnLightDelayed), launchLightDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchLightNotification);
        }
    }

    public void TryLaunchDecoy()
    {
        if (!upgradeActive) return;

        if (decoyCount > 0)
        {
            string slot = availableDecoySlots[availableDecoySlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, true, false);

            Invoke(nameof(SpawnDecoyDelayed), launchDecoyDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchDecoyNotification);
        }
    }

    private void SpawnLightDelayed()
    {
        var lightComponent = Instantiate(lightPrefab, lightSpawnTransform.position, lightSpawnTransform.rotation).GetComponent<DeployableLight>();
        lightComponent.gameObject.SetActive(true);

        lightComponent.LaunchWithForce(lightLaunchForce);
        lightComponent.GetComponentInChildren<CyclopsMaterialAssigner>().OnCyclopsReferenceFinished(CyclopsReferenceHandler.CyclopsReference);
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

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }

    public bool GetUpgradeActive() => upgradeActive;

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
}
