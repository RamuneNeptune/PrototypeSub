using ModStructureFormat;
using Newtonsoft.Json;
using PrototypeSubMod.MiscMonobehaviors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Facilities;

internal class SpawnSHProps : MonoBehaviour
{
    [SerializeField] private TextAsset[] structures;
    [SerializeField] private Transform parent;
    [Tooltip("Remove large world entity components")]
    [SerializeField] private bool removeLWEs = true;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(SpawnStructures());
    }

    private IEnumerator SpawnStructures()
    {
        foreach (var obj in structures)
        {
            var structure = JsonConvert.DeserializeObject<Structure>(obj.text);
            Dictionary<string, List<Entity>> entities = new();
            foreach (var entity in structure.Entities)
            {
                if (entity == null || entity.classId == null) continue;

                if (!entities.ContainsKey(entity.classId))
                {
                    entities.Add(entity.classId, new List<Entity>());
                }

                var entityList = entities[entity.classId];
                if (!entityList.Contains(entity))
                {
                    entityList.Add(entity);
                }
            }

            yield return SpawnEntities(entities);
        }
    }

    private IEnumerator SpawnEntities(Dictionary<string, List<Entity>> entities)
    {
        foreach (var classID in entities.Keys)
        {
            var prefabTask = PrefabDatabase.GetPrefabAsync(classID);
            yield return prefabTask;

            GameObject prefab = null;
            if (!prefabTask.TryGetPrefab(out prefab)) throw new System.Exception($"Error loading prefab with class ID of {classID}");

            prefab.SetActive(false);
            var lwe = prefab.GetComponent<LargeWorldEntity>();
            if (lwe) lwe.enabled = false;

            foreach (var entityData in entities[classID])
            {
                var pos = new Vector3(entityData.position.x, entityData.position.y, entityData.position.z);
                var rot = new Quaternion(entityData.rotation.x, entityData.rotation.y, entityData.rotation.z, entityData.rotation.w);
                var instance = GameObject.Instantiate(prefab, parent);
                instance.transform.position = pos;
                instance.transform.rotation = rot;

                if (removeLWEs)
                {
                    RemovePrefabComponents(instance);
                }

                instance.SetActive(true);
            }

            prefab.SetActive(true);
            if (lwe) lwe.enabled = true;
        }
    }

    private void RemovePrefabComponents(GameObject instance)
    {
        DestroyImmediate(instance.GetComponent<LargeWorldEntity>());
    }
}
