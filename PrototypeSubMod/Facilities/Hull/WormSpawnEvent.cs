using System;
using System.Collections;
using PrototypeSubMod.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PrototypeSubMod.Facilities.Hull;

public class WormSpawnEvent : MonoBehaviour
{
    private const float MIN_TIME_BETWEEN_SPAWNS = 120;
    
    [SaveStateReference(float.MinValue)]
    private static float _timeNextSpawn = float.MinValue;

    [SaveStateReference]
    private static GameObject _digInFX;
    [SaveStateReference]
    private static GameObject _digOutFX;

    [SerializeField] private ProtoWormAnimator wormAnimator;
    [SerializeField] private GameObject disableObjects;
    [SerializeField] private Transform raycastOrigin;
    
    private bool spawnedDigOutParticles;
    private bool wormActive;
    private int particleCount;
    private float timeNextParticles = float.MinValue;
    private float timeSpawned;

    private void Awake()
    {
        gameObject.SetActive(true);
        disableObjects.SetActive(false);
        UWE.CoroutineHost.StartCoroutine(TryRetrieveFXPrefabs());
    }

    private void Update()
    {
        if (transform.parent?.parent?.name == "Nautilus.PrefabCache") return;
        
        float time = Time.time + Random.Range(-0.01f, 0.01f);
        disableObjects.SetActive(time >= _timeNextSpawn || wormActive);
        if (time >= _timeNextSpawn && !wormActive)
        {
            _timeNextSpawn = time + MIN_TIME_BETWEEN_SPAWNS;
            wormActive = true;
        }
        else if (!wormActive)
        {
            return;
        }
        
        /*
        bool hitTerrain = Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit,
            2f, 1 << LayerID.TerrainCollider);
        if (!hitTerrain)
        {
            hitTerrain |= Physics.Raycast(raycastOrigin.position + raycastOrigin.forward, -raycastOrigin.forward, out hit,
                5f, 1 << LayerID.TerrainCollider);
        }
        */

        /*
        if (!hitTerrain)
        {
            Vector3 offset = -raycastOrigin.forward * LargeWorldStreamer.main.blocksPerTree;
            float maxDist = LargeWorldStreamer.main.blocksPerTree * 4;
            bool hitFront = AvoidTerrain.OctreeRaycastSkipCurrent(raycastOrigin.position + offset, raycastOrigin.forward, maxDist);
            bool hitBack = AvoidTerrain.OctreeRaycastSkipCurrent(raycastOrigin.position - offset, -raycastOrigin.forward, maxDist);
            hit.point = raycastOrigin.position + offset * (hitBack ? 1 : -1);
            hitTerrain |= hitFront || hitBack;
        }
        */

        var main = LargeWorldStreamer.main;
        Vector3 jitter = Random.onUnitSphere * 3;
        
        bool hit1 = !main.streamerV2.octreesStreamer.GetOctree(main.GetBlock(raycastOrigin.position + jitter) / main.blocksPerTree).IsEmpty();
        bool hit2 = !main.streamerV2.octreesStreamer.GetOctree(main.GetBlock(raycastOrigin.position - jitter) / main.blocksPerTree).IsEmpty();
        bool hitTerrain = hit1 && hit2;
        
        if (hitTerrain && Time.time > timeNextParticles && wormAnimator.GetNormalizedProgress() > 0.25f)
        {
            if (particleCount <= 6)
            {
                ErrorMessage.AddError($"Hit terrain at {raycastOrigin.position}");
                UWE.CoroutineHost.StartCoroutine(SpawnPrefabRepeating(_digInFX, raycastOrigin.position, 
                    wormAnimator.GetTimeForWormLength(), 0.5f));
            
                timeNextParticles = Time.time + 1f;
            }
        }

        if (!spawnedDigOutParticles && (raycastOrigin.position - transform.position).sqrMagnitude < 25 && Time.time > timeSpawned + 0.2f)
        {
            UWE.CoroutineHost.StartCoroutine(SpawnPrefabRepeating(_digOutFX, raycastOrigin.position,
                wormAnimator.GetTimeForWormLength(), 0.5f));
            
            spawnedDigOutParticles = true;
        }
    }

    private void OnEnable()
    {
        timeSpawned = Time.time;
    }

    private IEnumerator SpawnPrefabRepeating(GameObject prefab, Vector3 point, float totalDuration, float particleDuration)
    {
        particleCount++;
        for (int i = 0; i < Mathf.CeilToInt(totalDuration / particleDuration); i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var instance = Instantiate(prefab, point + Random.onUnitSphere * 2, Random.rotation);
                instance.transform.localScale = Vector3.one * 2f;
            }
            yield return new WaitForSeconds(particleDuration);
        }
    }

    private IEnumerator TryRetrieveFXPrefabs()
    {
        if (_digInFX && _digOutFX) yield break;

        var prefabTask = CraftData.GetPrefabForTechTypeAsync(TechType.Sandshark);
        yield return prefabTask;

        var prefab = prefabTask.GetResult();
        SandShark sandShark = prefab.GetComponent<SandShark>();
        _digInFX = sandShark.digInEffect;
        _digOutFX = sandShark.digOutEffect;
    }
}