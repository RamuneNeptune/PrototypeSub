using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Compatibility;
using System.IO;
using System.Threading;
using UnityEngine;

namespace PrototypeSubMod.Utility;

internal static class LoadEasyPrefabs
{
    public static void LoadPrefabs(AssetBundle assetBundle)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        foreach (var easyPrefab in assetBundle.LoadAllAssets<EasyPrefab>())
        {
            RegisterEasyPrefab(easyPrefab);
        }
        
        sw.Stop();
        Plugin.Logger.LogInfo($"Easy prefabs loaded in {sw.ElapsedMilliseconds}ms");
    }

    public static void RegisterEasyPrefab(EasyPrefab easyPrefab)
    {
        PrefabInfo info = PrefabInfo.WithTechType(easyPrefab.techType.techTypeName, null, null, unlockAtStart: easyPrefab.unlockAtStart);
        if (easyPrefab.sprite != null)
        {
            info = info.WithIcon(easyPrefab.sprite);
        }

        var prefab = new CustomPrefab(info); 
        if (easyPrefab.prefab)
        {
            prefab.SetGameObject(GetPrefab(easyPrefab));
        }

        if (easyPrefab.craftable)
        {
            string path = Path.Combine(easyPrefab.jsonRecipePath, $"{easyPrefab.techType.techTypeName}.json");
            prefab.SetRecipe(ROTACompatManager.GetRelevantRecipe(path));
        }

        if (easyPrefab.unlockAtStart)
        {
            prefab.SetUnlock(TechType.None);
        }

        if (easyPrefab.isProtoUpgrade && easyPrefab.includeInPDA)
        {
            prefab.SetPdaGroupCategory(Plugin.PrototypeGroup, Plugin.ProtoModuleCategory);
        }

        prefab.Register();
    }

    private static GameObject GetPrefab(EasyPrefab easyPrefab)
    {
        if (easyPrefab.applySNShaders)
        {
            if (easyPrefab.applyPrecursorMaterialChanges)
            {
                MaterialUtils.ApplySNShaders(easyPrefab.prefab, modifiers: new ProtoMaterialModifier(1));
            }
            else
            {
                MaterialUtils.ApplySNShaders(easyPrefab.prefab);
            }
        }

        if (easyPrefab.prefab.GetComponentsInChildren<Renderer>(true).Length > 0)
        {
            UWE.CoroutineHost.StartCoroutine(ProtoMatDatabase.ReplaceVanillaMats(easyPrefab.prefab));
        }
        
        return easyPrefab.prefab;
    }
}
