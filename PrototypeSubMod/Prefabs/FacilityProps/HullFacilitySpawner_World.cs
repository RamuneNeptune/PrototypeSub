using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

public class HullFacilitySpawner_World
{
    public static PrefabInfo prefabInfo;
    
    public static void Register()
    {
        var prefabInfo = PrefabInfo.WithTechType("ProtoHullFacilitySpawner",null, null, "English");

        var prefab = new CustomPrefab(prefabInfo);

        prefab.SetGameObject(GetGameObject);
        prefab.SetSpawns(new SpawnLocation(Vector3.zero));
        
        prefab.Register();
    }

    private static GameObject GetGameObject()
    {
        var asset = Plugin.AssetBundle.LoadAsset<GameObject>("HullFacilitySpawner");
        return GameObject.Instantiate(asset);
    }
}