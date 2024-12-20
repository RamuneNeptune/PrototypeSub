using Nautilus.Assets;
using Nautilus.Utility;
using PrototypeSubMod.MiscMonobehaviors;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

internal class FacilityPing
{
    public static PrefabInfo CreatePing(string techType, PingType pingType, ColorOverrideData colorOverride = null)
    {
        var prefabInfo = PrefabInfo.WithTechType(techType);

        var prefab = new CustomPrefab(prefabInfo);
        prefab.SetGameObject(GetGameObject(techType, prefabInfo, colorOverride));
        prefab.Register();

        return prefabInfo;
    }

    private static GameObject GetGameObject(string name, PrefabInfo info, ColorOverrideData colorOverride)
    {
        var empty = Plugin.AssetBundle.LoadAsset<GameObject>("Empty");
        empty.name = name;

        PrefabUtils.AddBasicComponents(empty, info.ClassID, info.TechType, LargeWorldEntity.CellLevel.Global);

        var pingInstance = empty.EnsureComponent<PingInstance>();
        pingInstance.pingType = Plugin.EngineFacilityPingType;
        pingInstance.origin = empty.transform;
        pingInstance.displayPingInManager = false;
        pingInstance.minDist = 25;
        pingInstance.range = 10;
        pingInstance.visitable = true;
        pingInstance.visitDistance = 100;
        pingInstance.visitDuration = 2f;
        pingInstance.SetColor(3);

        var pingSetter = empty.AddComponent<DelayedPingLabelSetter>();

        pingSetter.translationKey = info.TechType.ToString();
        pingSetter.pingInstance = pingInstance;

        var signalPing = empty.AddComponent<SignalPing>();
        signalPing.pingInstance = pingInstance;
        signalPing.disableOnEnter = true;
        signalPing.descriptionKey = info.TechType.ToString();

        var col = empty.AddComponent<SphereCollider>();
        col.radius = 10;
        col.isTrigger = true;

        if (colorOverride != null)
        {
            empty.AddComponent<PingColorOverride>().overrideColor = colorOverride.overrideColor;
        }

        return empty;
    }

    public class ColorOverrideData
    {
        public Color overrideColor;

        public ColorOverrideData(Color overrideColor)
        {
            this.overrideColor = overrideColor;
        }
    }
}
