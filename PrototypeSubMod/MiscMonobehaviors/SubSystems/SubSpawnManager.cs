using System;
using System.Collections;
using Nautilus.Utility;
using PrototypeSubMod.DestructionEvent;
using PrototypeSubMod.Facilities.Hull;
using PrototypeSubMod.Prefabs;
using Story;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class SubSpawnManager : MonoBehaviour
{
    [SerializeField] private AdditiveSceneSpawner sceneSpawner;

    private void Awake()
    {
        if (StoryGoalManager.main.IsGoalComplete("PrototypeSpawned"))
        {
            sceneSpawner.CancelLoad();
        }
    }

    private void Start()
    {
        sceneSpawner.onSceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded()
    {
        var sub = GameObject.Find("PrototypeSub-MainPrefab");
        sub.name = "PrototypeSub(Clone)";
        sub.transform.position = new Vector3(0, 500, 0);
        sub.GetComponentInChildren<ProtoDestructionEvent>().DestroySubNoSequence();
        UWE.CoroutineHost.StartCoroutine(SetupMaterials(sub));
        
        StoryGoalManager.main.OnGoalComplete("PrototypeSpawned");
        sub.SetActive(false);
    }

    private IEnumerator SetupMaterials(GameObject prefab)
    {
        yield return Prototype_Craftable.SetupProtoGameObject(prefab);
        
        yield return new WaitForEndOfFrame();
        prefab.GetComponent<VFXConstructing>().ghostMaterial = MaterialUtils.GhostMaterial;
        yield return new WaitForEndOfFrame();
    }
}