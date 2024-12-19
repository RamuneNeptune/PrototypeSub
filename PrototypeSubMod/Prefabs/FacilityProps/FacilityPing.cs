using Nautilus.Assets;
using Nautilus.Handlers;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class FacilityPing
{
    public static void CreatePing(string techType, PingType pingType, Vector3 spawnPos)
    {
        var prefabInfo = PrefabInfo.WithTechType(techType);

        var prefab = new CustomPrefab(prefabInfo);
        prefab.SetGameObject(GetGameObject(techType));
        prefab.Register();

        CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(prefabInfo.TechType, spawnPos));
    }

    private static GameObject GetGameObject(string name)
    {
        var empty = new GameObject();
        empty.name = name;

        var pingInstance = empty.EnsureComponent<PingInstance>();
        pingInstance.pingType = Plugin.EngineFacilityPingType;
        pingInstance.origin = empty.transform;
        pingInstance.displayPingInManager = false;
        pingInstance.range = 25;
        pingInstance.visitable = true;
        pingInstance.visitDistance = 25;
        pingInstance.visitDuration = 5f;
        pingInstance.SetColor(3);

        return empty;
    }
}
