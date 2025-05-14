using System;
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
    [SerializeField] private Transform wormHead;
    private bool wormActive;

    private void Awake()
    {
        gameObject.SetActive(true);
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
        var coord = (Int3)LargeWorldStreamer.main.transform.InverseTransformPoint(transform.position);
        var octree = LargeWorldStreamer.main.streamerV2.octreesStreamer.GetOctree(coord);
        bool hitTerrain = octree != null && !octree.IsEmpty();
        ErrorMessage.AddError($"Hit terrain = {hitTerrain}");
        */
    }
}