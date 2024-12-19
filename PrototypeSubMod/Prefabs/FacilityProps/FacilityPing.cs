using Nautilus.Assets;
using Nautilus.Utility;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class FacilityPing
{
    public static PrefabInfo CreatePing(string techType, PingType pingType)
    {
        var prefabInfo = PrefabInfo.WithTechType(techType);

        var prefab = new CustomPrefab(prefabInfo);
        prefab.SetGameObject(GetGameObject(techType, prefabInfo));
        prefab.Register();

        return prefabInfo;
    }

    private static GameObject GetGameObject(string name, PrefabInfo info)
    {
        var empty = Plugin.AssetBundle.LoadAsset<GameObject>("Empty");
        empty.name = name;

        PrefabUtils.AddBasicComponents(empty, info.ClassID, info.TechType, LargeWorldEntity.CellLevel.Global);

        var pingInstance = empty.EnsureComponent<PingInstance>();
        pingInstance.pingType = Plugin.EngineFacilityPingType;
        pingInstance.origin = empty.transform;
        pingInstance.displayPingInManager = false;
        pingInstance.range = 25;
        pingInstance.visitable = true;
        pingInstance.visitDistance = 25;
        pingInstance.visitDuration = 5f;
        pingInstance.SetLabel(info.TechType.ToString());
        pingInstance.SetColor(3);

        return empty;
    }
}
