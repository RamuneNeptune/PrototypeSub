using System;
using System.Collections;
using System.Collections.Generic;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class WormLightningManager : MonoBehaviour
{
    [SaveStateReference]
    private static GameObject _electricArcPrefab;

    [SerializeField] private ProtoWormSpineManager spineManager;
    [SerializeField] private Transform segmentsParent;

    private List<VFXElectricArcs> electricArcs = new();
    
    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(RetrievePrefab());
        UWE.CoroutineHost.StartCoroutine(InitializeArcs());
    }

    private IEnumerator RetrievePrefab()
    {
        if (_electricArcPrefab) yield break;

        var task = UWE.PrefabDatabase.GetPrefabAsync("e8143977-448e-4202-b780-83485fa5f31a");
        yield return task;

        if (!task.TryGetPrefab(out var antechamberPrefab))
            throw new System.Exception("Error loading antechamber prefab");

        var vfxController = antechamberPrefab.GetComponent<VFXController>();
        _electricArcPrefab = vfxController.emitters[0].fx;
    }

    private IEnumerator InitializeArcs()
    {
        yield return new WaitUntil(() => spineManager.GetSpawned() && _electricArcPrefab);

        SpawnArcs();
    }

    private void SpawnArcs()
    {
        for (int i = 0; i < segmentsParent.childCount - 1; i++)
        {
            var child1 = segmentsParent.GetChild(i);
            var child2 = segmentsParent.GetChild(i + 1);
            var instance = Instantiate(_electricArcPrefab, child1.position, Quaternion.identity, child1);
            var electricArc = instance.GetComponent<VFXElectricArcs>();
            electricArc.target = child2;
            electricArc.Play();
            electricArcs.Add(electricArc);
        }
    }

    private void OnEnable()
    {
        foreach (var arc in electricArcs)
        {
            arc.Play();
        }
    }
    
    private void OnDisable()
    {
        foreach (var arc in electricArcs)
        {
            arc.Stop();
        }
    }
}