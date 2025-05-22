using System.Collections;
using System.Security.Cryptography;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class WormRockSpawner : MonoBehaviour
{
    [SaveStateReference]
    private static GameObject _rock1Prefab;
    
    [SerializeField] private ProtoWormSpineManager spineManager;
    [SerializeField] private Transform spineParent;

    private Transform lastSpine;
    private bool hadHitTerrain;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(RetrieveLastSpine());
        UWE.CoroutineHost.StartCoroutine(RetrieveRockPrefab());
    }

    private IEnumerator RetrieveLastSpine()
    {
        yield return new WaitUntil(() => spineManager.GetSpawned());

        lastSpine = spineParent.GetChild(0);
    }

    private IEnumerator RetrieveRockPrefab()
    {
        if (_rock1Prefab) yield break;

        var task = UWE.PrefabDatabase.GetPrefabAsync("8d13d081-431e-4ed5-bc99-2b8b9fabe9c2");
        yield return task;

        if (!task.TryGetPrefab(out _rock1Prefab)) throw new System.Exception("Problem loading rock01 prefab");
    }

    private void Update()
    {
        if (!lastSpine) return;
        
        bool hitTerrain = Physics.Raycast(lastSpine.position - lastSpine.forward, -lastSpine.forward, out RaycastHit hit,
            6f, 1 << LayerID.TerrainCollider);
        if (!hitTerrain)
        {
            hitTerrain |= Physics.Raycast(lastSpine.position - lastSpine.forward * 6, lastSpine.forward, out hit,
                4f, 1 << LayerID.TerrainCollider);
        }

        if (hadHitTerrain != hitTerrain && hitTerrain)
        {
            SpawnRocks(hit.point, hit.normal);
        }
        
        hadHitTerrain = hitTerrain;
    }

    private void SpawnRocks(Vector3 position, Vector3 normal)
    {
        var instance = Instantiate(_rock1Prefab, position + Random.insideUnitSphere, Quaternion.LookRotation(normal));
        DestroyImmediate(instance.GetComponent<PrefabIdentifier>());
        StartCoroutine(SizeUpRock(instance, new Vector3(1.5f, 1.5f, 0.75f), 0.5f));
    }

    private IEnumerator SizeUpRock(GameObject rock, Vector3 targetScale, float duration)
    {
        float progress = 0;
        while (progress < duration)
        {
            progress += Time.deltaTime;
            rock.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress / duration);            
            yield return null;
        }
    }
}