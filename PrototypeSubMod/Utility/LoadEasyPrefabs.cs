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
    private static int totalPrefabCount;
    private static int registeredPrefabCount;
    
    public static void LoadPrefabs(AssetBundle assetBundle, params Action[] onCompleted)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        totalPrefabCount = 0;
        registeredPrefabCount = 0;
        
        foreach (var easyPrefab in assetBundle.LoadAllAssets<EasyPrefab>())
        {
            RegisterEasyPrefab(easyPrefab, onCompleted);
            totalPrefabCount++;
        }
        
        sw.Stop();
        Plugin.Logger.LogInfo($"Easy prefabs loaded in {sw.ElapsedMilliseconds}ms");
    }

    public static void RegisterEasyPrefab(EasyPrefab easyPrefab, Action[] onCompleted)
    {
        PrefabInfo info = PrefabInfo.WithTechType(easyPrefab.techType.techTypeName, null, null, unlockAtStart: easyPrefab.unlockAtStart);
        if (easyPrefab.sprite != null)
        {
            info = info.WithIcon(easyPrefab.sprite);
        }

        var prefab = new CustomPrefab(info); 
        if (easyPrefab.prefab)
        {
            UWE.CoroutineHost.StartCoroutine(RegisterPrefabWithObject(easyPrefab, prefab, onCompleted));
        }
        else
        {
            SetupMiscellaneousValues(easyPrefab, prefab);

            prefab.Register();

            registeredPrefabCount++;
            if (registeredPrefabCount >= totalPrefabCount)
            {
                foreach (var action in onCompleted)
                {
                    action?.Invoke();
                }
            }
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

        if (easyPrefab.isProtoUpgrade && easyPrefab.includeInPDA)
        {
            prefab.SetPdaGroupCategory(Plugin.PrototypeGroup, Plugin.ProtoModuleCategory);
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
        Plugin.Logger.LogInfo($"Prefab registered for {easyPrefab.prefab} | Current registered count = {registeredPrefabCount} / {totalPrefabCount}");
        
        registeredPrefabCount++;
        if (registeredPrefabCount >= totalPrefabCount)
        {
            foreach (var action in onCompleted)
            {
                action?.Invoke();
            }
        }
    }
}
