using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using PrototypeSubMod.MiscMonobehaviors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class CustomPing
{
    private static List<Type> WhitelistedComponents = new()
    {
        typeof(Transform),
        typeof(PrefabIdentifier),
        typeof(TechTag),
        typeof(LargeWorldEntity),
        typeof(PingInstance)
    };

    public static void CreatePing(string techType, PingType pingType, Vector3 spawnPos, Color colorOverride = default, float minDist = 25, float fadeRange = 10)
    {
        var prefabInfo = PrefabInfo.WithTechType(techType);

        var prefab = new CustomPrefab(prefabInfo);
        var cloneTemplate = new CloneTemplate(prefabInfo, TechType.Beacon);
        cloneTemplate.ModifyPrefab += gameObject =>
        {
            SetupGameObject(ref gameObject, prefabInfo, pingType, colorOverride, minDist, fadeRange);
        };

        prefab.SetGameObject(cloneTemplate);
        prefab.SetSpawns(new SpawnLocation[]
        {
            new SpawnLocation(spawnPos)
        });

        prefab.Register();
    }

    private static void SetupGameObject(ref GameObject beacon, PrefabInfo info, PingType pingType, Color colorOverride, float minDist, float fadeRange)
    {
        beacon.name = info.TechType.ToString();

        foreach (var item in beacon.GetComponents<Component>())
        {
            if (!WhitelistedComponents.Contains(item.GetType()))
            {
                UnityEngine.Object.DestroyImmediate(item);
            }
        }

        foreach (Transform child in beacon.transform)
        {
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }

        var pingInstance = beacon.EnsureComponent<PingInstance>();
        pingInstance.pingType = pingType;
        pingInstance.origin = beacon.transform;
        pingInstance.displayPingInManager = false;
        pingInstance.minDist = minDist;
        pingInstance.range = fadeRange;
        pingInstance.visitable = true;
        pingInstance.visitDistance = 100;
        pingInstance.visitDuration = 2f;
        pingInstance.SetColor(0);

        var pingSetter = beacon.EnsureComponent<DelayedPingLabelSetter>();

        pingSetter.translationKey = info.TechType.ToString();
        pingSetter.pingInstance = pingInstance;

        var signalPing = beacon.EnsureComponent<SignalPing>();
        signalPing.pingInstance = pingInstance;
        signalPing.disableOnEnter = true;
        signalPing.descriptionKey = info.TechType.ToString();

        var col = beacon.EnsureComponent<SphereCollider>();
        col.radius = 10;
        col.isTrigger = true;

        if (colorOverride != default)
        {
            beacon.EnsureComponent<PingColorOverride>().overrideColor = colorOverride;
        }
    }
}
