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

    [SerializeField] private GameObject disableObjects;
    [SerializeField] private Transform raycastOrigin;
    
    private bool wormActive;

    private void Awake()
    {
        gameObject.SetActive(true);
        disableObjects.SetActive(false);
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
        
        bool hitTerrain = Physics.Raycast(raycastOrigin.position, raycastOrigin.forward,
            2f, 1 << LayerID.TerrainCollider);
        hitTerrain |= Physics.Raycast(raycastOrigin.position, -raycastOrigin.forward,
            2f, 1 << LayerID.TerrainCollider);
        ErrorMessage.AddError($"Hit terrain = {hitTerrain}");
    }
}