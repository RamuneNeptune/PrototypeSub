using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets;
using UnityEngine;
using UWE;
using System.Collections;
using System;
using System.Collections.Generic;
using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors;

namespace PrototypeSubMod.Prefabs;

internal class DisplayCaseProp
{
    public static void Register(string displayObjectClassID, string newTechType, TechType scanTechType, Vector3 localOffset)
    {
        var prefabInfo = PrefabInfo.WithTechType(newTechType, null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, "d0fea4da-39f2-47b4-aece-bb12fe7f9410");

        CraftData.PreparePrefabIDCache();
        cloneTemplate.ModifyPrefabAsync = gameObject =>
        {
            return ModifyPrefab(gameObject, displayObjectClassID, scanTechType, localOffset);
        };

        prefab.SetGameObject(cloneTemplate);

        prefab.Register();
    }

    private static IEnumerator ModifyPrefab(GameObject prefab, string displayObjectClassID, TechType scanTechType, Vector3 localOffset)
    {
        var prefabTask = PrefabDatabase.GetPrefabAsync(displayObjectClassID);
        yield return prefabTask;

        GameObject modelPrefab = null;
        if (!prefabTask.TryGetPrefab(out modelPrefab))
        {
            throw new Exception($"Error retrieving prefab with class ID = {displayObjectClassID}");
        }

        var instance = GameObject.Instantiate(modelPrefab, prefab.transform);
        TrimComponents(instance);

        if (prefab.TryGetComponent(out SkyApplier applier))
        {
            applier.renderers.AddRangeToArray(instance.GetComponentsInChildren<Renderer>());
        }

        var col = instance.AddComponent<CapsuleCollider>();
        col.radius = 1.5f / instance.transform.localScale.x;
        col.height = 6.3f / instance.transform.localScale.x;
        instance.transform.localPosition = localOffset;

        var tag = instance.AddComponent<TechTag>();
        tag.type = scanTechType;

        instance.AddComponent<MarkAsEntityRoot>();
    }

    private static readonly List<Type> whitelistedComponents = new()
    {
        typeof(Transform),
        typeof(PrefabIdentifier),
        typeof(Renderer),
        typeof(MeshFilter),
    };

    private static void TrimComponents(GameObject instance)
    {
        foreach (var component in instance.GetComponents<Component>())
        {
            if (!whitelistedComponents.Contains(component.GetType()))
            {
                GameObject.DestroyImmediate(component);
            }
        }
    }
}
