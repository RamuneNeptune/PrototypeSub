using PrototypeSubMod.MiscMonobehaviors.Materials;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

public class DestroyObjectAfterSpawn : MonoBehaviour
{
    [SerializeField] private Component objectSpawner;
    [SerializeField] private string[] childPaths;

    private void OnValidate()
    {
        if (objectSpawner is not IMaterialModifier) objectSpawner = null;
    }

    private void Start()
    {
        (objectSpawner as IMaterialModifier).onEditMaterial += OnObjectSpawned;
    }
    
    private void OnObjectSpawned(GameObject spawnedObj)
    {
        foreach (var path in childPaths)
        {
            var obj = spawnedObj.transform.Find(path);
            if (!obj)
            {
                Plugin.Logger.LogWarning($"Child not found at path {path} under object {spawnedObj}!");
                continue;
            }

            Destroy(obj.gameObject);
        }
    }
}