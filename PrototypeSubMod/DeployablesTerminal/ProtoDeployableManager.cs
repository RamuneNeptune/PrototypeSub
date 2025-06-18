using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.Upgrades;
using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.UI.AbilitySelection;
using UnityEngine;

namespace PrototypeSubMod.DeployablesTerminal;

internal class ProtoDeployableManager : ProtoUpgrade
{
    [SerializeField] DeployablesStorageTerminal storageTerminal;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private VoiceNotification launchLightNotification;
    [SerializeField] private VoiceNotification invalidOperationNotification;
    [SerializeField] private FMOD_CustomEmitter deployLightSFX;
    [SerializeField] private GameObject lightPrefab;
    [SerializeField] private Transform lightSpawnTransform;
    [SerializeField] private GenericRadialAbility lightAbility;
    
    [SerializeField] private float launchLightDelay;
    [SerializeField] private float lightLaunchForce;

    private int lightCount;
    private List<string> availableLightSlots = new();

    public void TryLaunchLight()
    {
        if (ionGenerator.GetUpgradeEnabled() && ionGenerator.GetUpgradeInstalled())
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(invalidOperationNotification);
            return;
        }

        if (lightCount > 0)
        {
            string slot = availableLightSlots[availableLightSlots.Count - 1];
            storageTerminal.equipment.RemoveItem(slot, true, false);

            Invoke(nameof(SpawnLightDelayed), launchLightDelay);

            subRoot.voiceNotificationManager.PlayVoiceNotification(launchLightNotification);
            deployLightSFX.Play();
        }
        else
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(invalidOperationNotification);
            lightAbility.SetQueuedActivationFailure();
        }
    }

    private void SpawnLightDelayed()
    {
        var lightComponent = Instantiate(lightPrefab, lightSpawnTransform.position, lightSpawnTransform.rotation).GetComponent<DeployableLight>();
        lightComponent.gameObject.SetActive(true);

        lightComponent.LaunchWithForce(lightLaunchForce, subRoot.rb.velocity);
    }

    public void RecalculateDeployableTotals()
    {
        lightCount = 0;

        foreach (var slot in DeployablesStorageTerminal.LightBeaconSlots)
        {
            var item = storageTerminal.equipment.GetItemInSlot(slot);

            if (item != null)
            {
                availableLightSlots.Add(slot);
                lightCount++;
            }
        }
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;

    // The deployable manager will be called via different icons
    public override bool OnActivated() => false;
    public override void OnSelectedChanged(bool changed) { }
}
