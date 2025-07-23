using System;
using System.Linq;
using HarmonyLib;
using Nautilus.Extensions;
using PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class AssignFloodlightRef : MonoBehaviour
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private SpawnPrefabAtRuntime floodlightSpawner;

    private void Start()
    {
        floodlightSpawner.onEditMaterial += OnObjectSpawned;
    }

    private void OnObjectSpawned(GameObject obj)
    {
        var list = subRoot.dimFloodlightsOnEnter.ToList();
        var light = GetComponentInChildren<VFXVolumetricLight>(true);
        list.Add(light);
        subRoot.dimFloodlightsOnEnter = list.ToArray();
        light.DisableVolume();
    }
}