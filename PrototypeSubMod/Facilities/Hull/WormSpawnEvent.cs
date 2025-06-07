using System;
using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.Utility;
using Story;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PrototypeSubMod.Facilities.Hull;

public class WormSpawnEvent : MonoBehaviour
{
    private const float MIN_TIME_BETWEEN_SPAWNS = 120;
    
    [SaveStateReference(float.MinValue)]
    private static float _timeNextSpawn = float.MinValue;

    [SaveStateReference]
    private static List<Vector3> _activeSpawnLocations;
    
    [SaveStateReference]
    private static GameObject _digInFX;
    [SaveStateReference]
    private static GameObject _digOutFX;

    [SerializeField] private ProtoWormAnimator wormAnimator;
    [SerializeField] private GameObject disableObjects;
    [SerializeField] private Transform raycastOrigin;
    
    [Header("SFX")]
    [SerializeField] private FMOD_CustomEmitter breachSurfaceSFX;
    [SerializeField] private FMOD_CustomLoopingEmitter swimLoopSFX;
    [SerializeField] private FMOD_CustomLoopingEmitter rumbleFarSFX;
    [SerializeField] private FMOD_CustomLoopingEmitter rumbleCloseSFX;
    
    private bool spawnedDigOutParticles;
    private bool wormActive;
    private bool hasSpawned;
    private bool calledDestroy;
    private int particleCount;
    private float timeNextParticles = float.MinValue;
    private float timeSpawned;
    private float timeOffset;

    private void Awake()
    {
        disableObjects.SetActive(false);
        UWE.CoroutineHost.StartCoroutine(TryRetrieveFXPrefabs());
    }

    private IEnumerator Start()
    {
        timeOffset = (float)gameObject.GetHashCode() / int.MaxValue;

        yield return new WaitForEndOfFrame();
        
        if (_activeSpawnLocations == null) _activeSpawnLocations = new();

        if (_activeSpawnLocations.Contains(transform.position))
        {
            Plugin.Logger.LogInfo($"Spawn locations contains {transform.position} | Destroying {gameObject}");
            Destroy(gameObject);
            calledDestroy = true;
            yield break;
        }
        
        _activeSpawnLocations.Add(transform.position);
    }

    private void Update()
    {
        if (!StoryGoalManager.main.IsGoalComplete("HullFacilityActivateWorm")) return;

        if (calledDestroy) return;
        
        float time = Time.time + timeOffset;
        disableObjects.SetActive(time >= _timeNextSpawn || wormActive);
        if (time >= _timeNextSpawn && !wormActive && !hasSpawned)
        {
            _timeNextSpawn = time + MIN_TIME_BETWEEN_SPAWNS;
            wormActive = true;
            hasSpawned = true;
        }
        else if (!wormActive && !calledDestroy)
        {
            swimLoopSFX.Stop();
            breachSurfaceSFX.Stop();
            rumbleFarSFX.Stop();
            rumbleCloseSFX.Stop();
            Destroy(gameObject, 5f);
            calledDestroy = true;
            return;
        }

        var main = LargeWorldStreamer.main;
        Vector3 jitter = Random.onUnitSphere * 3;

        var cell1 = main.streamerV2.octreesStreamer.
            GetOctree(main.GetBlock(raycastOrigin.position + jitter) / main.blocksPerTree);
        var cell2 = main.streamerV2.octreesStreamer.
                GetOctree(main.GetBlock(raycastOrigin.position - jitter) / main.blocksPerTree);
        bool hit1 = cell1 != null && !cell1.IsEmpty();
        bool hit2 = cell2 != null && !cell2.IsEmpty();
        bool hitTerrain = hit1 && hit2;
        
        if (hitTerrain && Time.time > timeNextParticles && wormAnimator.GetNormalizedProgress() > 0.25f)
        {
            const int maxParticleCount = 6;
            if (particleCount <= maxParticleCount)
            {
                float normalizedParticleCount = (float)particleCount / maxParticleCount;
                float particleDuration = Mathf.Lerp(wormAnimator.GetTimeForWormLength() / 1.5f,
                    wormAnimator.GetTimeForWormLength(),Mathf.InverseLerp(0.5f, 1f, normalizedParticleCount));
                
                UWE.CoroutineHost.StartCoroutine(SpawnPrefabRepeating(_digInFX, raycastOrigin.position, particleDuration,
                    0.5f));
            
                timeNextParticles = Time.time + Mathf.Lerp(1f, 3f, normalizedParticleCount);
            }
        }

        if (wormAnimator.GetNormalizedProgress() >= 1f)
        {
            wormActive = false;
        }

        float sqrDistToHead = (raycastOrigin.position - transform.position).sqrMagnitude;
        if (!spawnedDigOutParticles && sqrDistToHead < 25 && Time.time > timeSpawned + 0.2f)
        {
            UWE.CoroutineHost.StartCoroutine(SpawnPrefabRepeating(_digOutFX, raycastOrigin.position,
                wormAnimator.GetTimeForWormLength(), 0.5f));
            breachSurfaceSFX.Play();
            swimLoopSFX.Play();
            
            spawnedDigOutParticles = true;
        }

        HandleRumbleAudio(sqrDistToHead);
    }

    private void HandleRumbleAudio(float sqrDistToHead)
    {
        const float farDistThreshold = 30 * 30;
        if (sqrDistToHead > farDistThreshold)
        {
            rumbleFarSFX.Play();
            rumbleCloseSFX.Stop();
        }
        else
        {
            rumbleCloseSFX.Play();
            rumbleFarSFX.Stop();
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

        particleCount--;
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
    
    private void OnDestroy()
    {
        _activeSpawnLocations.Remove(transform.position);
    }

    public static void ResetSpawnTimer()
    {
        _timeNextSpawn = 0;
    }
}