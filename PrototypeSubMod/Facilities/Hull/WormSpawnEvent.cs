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

    private bool inPrefabCache;
    private bool spawnedDigOutParticles;
    private bool wormActive;
    private bool hasSpawned;
    private int particleCount;
    private float timeNextParticles = float.MinValue;
    private float timeSpawned;
    private float timeOffset;

    private void Awake()
    {
        disableObjects.SetActive(false);
        UWE.CoroutineHost.StartCoroutine(TryRetrieveFXPrefabs());
    }

    private void Start()
    {
        inPrefabCache = transform.parent && transform.parent.parent && transform.parent.parent.name == "Nautilus.PrefabCache";
        gameObject.SetActive(!inPrefabCache);
        timeOffset = (float)gameObject.GetHashCode() / int.MaxValue;
    }

    private void Update()
    {
        if (inPrefabCache) return;

        float time = Time.time + timeOffset;
        disableObjects.SetActive(time >= _timeNextSpawn || wormActive);
        if (time >= _timeNextSpawn && !wormActive && !hasSpawned)
        {
            _timeNextSpawn = time + MIN_TIME_BETWEEN_SPAWNS;
            wormActive = true;
            hasSpawned = true;
        }
        else if (!wormActive)
        {
            return;
        }

        var main = LargeWorldStreamer.main;
        Vector3 jitter = Random.onUnitSphere * 3;

        var cell1 = main.streamerV2.octreesStreamer.GetOctree(main.GetBlock(raycastOrigin.position + jitter) /
                                                               main.blocksPerTree);
        var cell2 =
            main.streamerV2.octreesStreamer.GetOctree(main.GetBlock(raycastOrigin.position - jitter) /
                                                      main.blocksPerTree);
        bool hit1 = cell1 != null && !cell1.IsEmpty();
        bool hit2 = cell2 != null && !cell2.IsEmpty();
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

        if (wormAnimator.GetNormalizedProgress() >= 1f)
        {
            wormActive = false;
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

    public static void ResetSpawnTimer()
    {
        _timeNextSpawn = 0;
    }
}