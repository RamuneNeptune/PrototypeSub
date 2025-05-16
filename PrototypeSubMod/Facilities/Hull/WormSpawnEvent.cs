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
    
    private bool wormActive;
    private bool hadHitTerrain;

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
        
        bool hitTerrain = Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit,
            2f, 1 << LayerID.TerrainCollider);
        if (!hitTerrain)
        {
            hitTerrain |= Physics.Raycast(raycastOrigin.position + raycastOrigin.forward, -raycastOrigin.forward, out hit,
                5f, 1 << LayerID.TerrainCollider);
        }
        
        if (hitTerrain != hadHitTerrain && hitTerrain)
        {
            ErrorMessage.AddError($"Hit terrain at {hit.point}");
            if (wormAnimator.GetNormalizedProgress() < 0.25f)
            {
                UWE.CoroutineHost.StartCoroutine(SpawnPrefabRepeating(_digOutFX, hit.point,
                    wormAnimator.GetTimeForWormLength(), 0.5f));
            }
            else
            {
                UWE.CoroutineHost.StartCoroutine(SpawnPrefabRepeating(_digInFX, hit.point,
                    wormAnimator.GetTimeForWormLength(), 0.5f));
            }
        }
        
        hadHitTerrain = hitTerrain;
    }

    private IEnumerator SpawnPrefabRepeating(GameObject prefab, Vector3 point, float totalDuration, float particleDuration)
    {
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