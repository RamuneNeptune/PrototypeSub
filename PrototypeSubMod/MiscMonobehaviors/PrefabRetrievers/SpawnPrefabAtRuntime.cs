using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

internal class SpawnPrefabAtRuntime : MonoBehaviour
{
    public event Action<GameObject> OnSpawn;

    [SerializeField] private string classID;
    [SerializeField] private string childPath;
    [SerializeField] private Material[] materials;
    [SerializeField] private Vector3 localPos;
    [SerializeField] private Vector3 localRot;
    [SerializeField] private Vector3 localScale = Vector3.one;

    private void Start()
    {
        CoroutineHost.StartCoroutine(SpawnPrefab());
    }

    private IEnumerator SpawnPrefab()
    {
        var task = PrefabDatabase.GetPrefabAsync(classID);
        yield return task;

        bool success = task.TryGetPrefab(out var prefab);
        if (!success)
        {
            throw new Exception($"Error loading prefab with class ID \"{classID}\"");
        }

        GameObject spawnObj = string.IsNullOrEmpty(childPath) ? prefab : prefab.transform.Find(childPath).gameObject;

        var worldPrefab = Instantiate(spawnObj, transform, false);
        worldPrefab.SetActive(true);
        worldPrefab.transform.localPosition = localPos;
        worldPrefab.transform.localEulerAngles = localRot;
        worldPrefab.transform.localScale = localScale;
        OnSpawn?.Invoke(spawnObj);

        if (worldPrefab.TryGetComponent(out Renderer rend) && materials.Length > 0)
        {
            rend.materials = materials;
        }

        var applier = GetComponentInParent<SkyApplier>();
        if (applier)
        {
            applier.renderers.AddRangeToArray(GetComponentsInChildren<Renderer>(true));
            applier.ApplySkybox();
        }
    }
}
