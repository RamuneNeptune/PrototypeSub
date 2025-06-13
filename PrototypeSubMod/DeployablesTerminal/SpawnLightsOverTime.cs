using PrototypeSubMod.Prefabs;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.DeployablesTerminal;

internal class SpawnLightsOverTime : MonoBehaviour
{
    private static GameObject DeployableLightPrefab;

    [SerializeField] private DeployablesStorageTerminal terminal;
    [SerializeField] private float timeBetweenSpawns;

    private float currentSpawnTimer;

    private IEnumerator Start()
    {
        if (DeployableLightPrefab != null) yield break;

        var prefabTask = PrefabDatabase.GetPrefabAsync(DeployableLight_Craftable.prefabInfo.ClassID);
        yield return prefabTask;

        if (!prefabTask.TryGetPrefab(out DeployableLightPrefab)) throw new System.Exception($"Error retriving deployable light prefab");
    }

    private void Update()
    {
        if (currentSpawnTimer < timeBetweenSpawns)
        {
            currentSpawnTimer += Time.deltaTime;
        }
        else
        {
            currentSpawnTimer = 0;
            SpawnLight();
        }
    }

    private void SpawnLight()
    {
        string freeSlot = string.Empty;
        if (!terminal.equipment.GetFreeSlot(Plugin.LightBeaconEquipmentType, out freeSlot)) return;

        var instance = Instantiate(DeployableLightPrefab);
        var pickupable = instance.GetComponent<Pickupable>();
        pickupable.SetInventoryItem(new InventoryItem(pickupable));

        instance.gameObject.SetActive(false);
        terminal.IgnoreSoundNextEquip();
        terminal.equipment.AddItem(freeSlot, pickupable.inventoryItem, true);
    }
}
