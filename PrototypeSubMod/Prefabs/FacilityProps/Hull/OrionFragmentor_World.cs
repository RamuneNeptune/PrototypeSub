using System;
using System.Collections;
using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Prefabs.FacilityProps.Hull;

public static class OrionFragmentor_World
{
    public static PrefabInfo prefabInfo;

    public static void Register()
    {
        prefabInfo = PrefabInfo.WithTechType("ProtoOrionFragmentor", null, null);
        
        var prefab = new CustomPrefab(prefabInfo);
        prefab.SetGameObject(GetPrefab);

        prefab.Register();
    }

    private static readonly List<Type> whitelistedComponents = new()
    {
        typeof(Transform),
        typeof(Renderer),
        typeof(MeshFilter),
        typeof(Collider),
        typeof(ConstructionObstacle)
    };
    
    private static readonly List<EntityData> decoProps2Data = new()
    {
        new EntityData(new Vector3(4.837f, 2.429f, 0.931f), Vector3.one * 0.805f, new Vector3(44.869f, 324.870f, 90)),
        new EntityData(new Vector3(-2.536f, 2.429f, -4.256f), Vector3.one * 0.805f, new Vector3(44.869f, 144.870f, 90.000f)),
        new EntityData(new Vector3(2.818f, 5.262f, -4.045f), Vector3.one * 0.805f, new Vector3(29.893f, 90.091f, 144.648f)),
        new EntityData(new Vector3(0.369f, 5.729f, -0.593f), new Vector3(1.069f, 0.805f, 0.805f), new Vector3(89.909f, 326.782f, 0))
    };

    private static readonly List<EntityData> surgicalMachineData = new()
    {
        new EntityData(new Vector3(1.388f, 2.415f, -1.484f), Vector3.one * 0.677f, new Vector3(46.983f, 144.870f, 90)),
        new EntityData(new Vector3(1.112f, 2.415f, -1.620f), Vector3.one * 0.677f, new Vector3(31.131f, 89.173f, 322.844f)),
        new EntityData(new Vector3(-0.521f, 2.786f, 0.722f), Vector3.one, new Vector3(0, 324.348f, 0))
    };
    
    private static IEnumerator GetPrefab(IOut<GameObject> prefabOut)
    {
        var empty = Plugin.AssetBundle.LoadAsset<GameObject>("Empty");
        var instance = GameObject.Instantiate(empty);

        PrefabUtils.AddBasicComponents(instance, prefabInfo.ClassID, prefabInfo.TechType,
            LargeWorldEntity.CellLevel.Far);
        
        var decoProps2Prefab = PrefabDatabase.GetPrefabAsync("3bbf8830-e34f-43a1-bbb3-743c7e6860ac");
        yield return decoProps2Prefab;

        if (!decoProps2Prefab.TryGetPrefab(out var decoProps2)) throw new Exception("Error loading decoProps2 prefab");

        foreach (var data in decoProps2Data)
        {
            var prop = GameObject.Instantiate(decoProps2, instance.transform);
            prop.transform.localPosition = data.position;
            prop.transform.localScale = data.scale;
            prop.transform.localEulerAngles = data.eulerAngles;
            DisplayCaseProp.TrimComponents(prop, whitelistedComponents);
        }

        var surgicalMachinePrefab = PrefabDatabase.GetPrefabAsync("6a01a336-fb46-469a-9f7d-1659e07d11d7");
        yield return surgicalMachinePrefab;

        if (!surgicalMachinePrefab.TryGetPrefab(out var surgicalMachine)) throw new Exception("Error loading surgicalMachine prefab");
        foreach (var data in surgicalMachineData)
        {
            var prop = GameObject.Instantiate(surgicalMachine, instance.transform);
            prop.transform.localPosition = data.position;
            prop.transform.localScale = data.scale;
            prop.transform.localEulerAngles = data.eulerAngles;
            DisplayCaseProp.TrimComponents(prop, whitelistedComponents);
        }

        var labTablePrefab = PrefabDatabase.GetPrefabAsync("df9aed66-c131-4570-9dcd-1e3d2109dcaa");
        yield return labTablePrefab;
        
        if (!labTablePrefab.TryGetPrefab(out var labTable)) throw new Exception("Error loading labTable prefab");
        
        var table = GameObject.Instantiate(labTable, instance.transform);
        table.transform.localPosition = Vector3.zero;
        table.transform.localScale = Vector3.one;
        table.transform.localEulerAngles = new Vector3(0, 324.568f, 0);
        DisplayCaseProp.TrimComponents(table, whitelistedComponents);
        
        var teleporterPrefab = PrefabDatabase.GetPrefabAsync("9a5ff289-4b82-4b9c-b9f6-d60f46dd2d7a");
        yield return teleporterPrefab;
        
        if (!teleporterPrefab.TryGetPrefab(out var teleporter)) throw new Exception("Error loading teleporter prefab");
        
        var teleporterInstance = GameObject.Instantiate(teleporter, instance.transform);
        teleporterInstance.transform.localPosition = new Vector3(1.167f, -3.328f, -1.717f);
        teleporterInstance.transform.localScale = Vector3.one * 0.837f;
        teleporterInstance.transform.localEulerAngles = new Vector3(0, 324.999f, 0);
        DisplayCaseProp.TrimComponents(teleporterInstance, whitelistedComponents);

        prefabOut.Set(instance);
    }

    private struct EntityData
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 eulerAngles;

        public EntityData(Vector3 position, Vector3 scale, Vector3 eulerAngles)
        {
            this.position = position;
            this.scale = scale;
            this.eulerAngles = eulerAngles;
        }
    }
}