using System;
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
    public static IEnumerator LoadPrefabs(AssetBundle assetBundle, params Action[] onCompleted)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        foreach (var easyPrefab in assetBundle.LoadAllAssets<EasyPrefab>())
        {
            yield return RegisterEasyPrefab(easyPrefab, onCompleted);
        }
        
        foreach (var action in onCompleted)
        {
            action?.Invoke();
        }
        
        sw.Stop();
        Plugin.Logger.LogInfo($"Easy prefabs fully started in {sw.ElapsedMilliseconds}ms");
    }

    public static IEnumerator RegisterEasyPrefab(EasyPrefab easyPrefab, Action[] onCompleted)
    {
        PrefabInfo info = PrefabInfo.WithTechType(easyPrefab.techType.techTypeName, null, null, unlockAtStart: easyPrefab.unlockAtStart);
        if (easyPrefab.sprite != null)
        {
            info = info.WithIcon(easyPrefab.sprite);
        }

        var prefab = new CustomPrefab(info); 
        if (easyPrefab.prefab)
        {
            yield return RegisterPrefabWithObject(easyPrefab, prefab, onCompleted);
        }
        else
        {
            SetupMiscellaneousValues(easyPrefab, prefab);

            prefab.Register();
        }
    }

    private static void SetupMiscellaneousValues(EasyPrefab easyPrefab, CustomPrefab prefab)
    {
        if (easyPrefab.craftable)
        {
            string path = Path.Combine(easyPrefab.jsonRecipePath, $"{easyPrefab.techType.techTypeName}.json");
            prefab.SetRecipe(ROTACompatManager.GetRelevantRecipe(path));
        }

        if (easyPrefab.unlockAtStart)
        {
            prefab.SetUnlock(TechType.None);
        }

        if (easyPrefab.isProtoUpgrade)
        {
            prefab.SetPdaGroupCategory(Plugin.PrototypeGroup, Plugin.ProtoModuleCategory);
        }
        else if (!string.IsNullOrEmpty(easyPrefab.techGroup) && !string.IsNullOrEmpty(easyPrefab.techCategory))
        {
            var techGroup = (TechGroup)Enum.Parse(typeof(TechGroup), easyPrefab.techGroup);
            var techCategory = (TechCategory)Enum.Parse(typeof(TechCategory), easyPrefab.techCategory);
            prefab.SetPdaGroupCategory(techGroup, techCategory);
        }
    }

    private static IEnumerator RegisterPrefabWithObject(EasyPrefab easyPrefab, CustomPrefab prefab, Action[] onCompleted)
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
            yield return ProtoMatDatabase.ReplaceVanillaMats(easyPrefab.prefab);
        }
        
        SetupMiscellaneousValues(easyPrefab, prefab);
        
        prefab.SetGameObject(easyPrefab.prefab);
        
        prefab.Register();
    }
}
