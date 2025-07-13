using Nautilus.Assets;
using Nautilus.Utility;
using PrototypeSubMod.Facilities;
using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.FacilityProps;

public static class StoryGoalTerminal
{
    public static void CreateTerminal(string techType, string storyGoalKey)
    {
        var prefabInfo = PrefabInfo.WithTechType(techType);

        var prefab = new CustomPrefab(prefabInfo);
        prefab.SetGameObject(GetGameObject(prefabInfo, storyGoalKey));

        prefab.Register();
    }

    private static GameObject GetGameObject(PrefabInfo prefabInfo, string storyKey)
    {
        var prefab = Plugin.AssetBundle.LoadAsset<GameObject>("Empty");
        
        var obj = UWE.Utils.InstantiateDeactivated(prefab);
        PrefabUtils.AddBasicComponents(obj, prefabInfo.ClassID, prefabInfo.TechType, LargeWorldEntity.CellLevel.Medium);

        var spawner = new GameObject("Spawner");
        spawner.transform.SetParent(obj.transform, false);
        var terminal = spawner.AddComponent<MultipurposeAlienTerminal>();
        spawner.AddComponent<UnlockStoryGoal>().ManualSetup(terminal, storyKey);
        obj.EnsureComponent<ApplyMaterialDatabase>();

        return obj;
    }
}