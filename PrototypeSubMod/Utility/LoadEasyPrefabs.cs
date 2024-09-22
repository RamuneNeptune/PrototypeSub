using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using System.IO;
using UnityEngine;

namespace PrototypeSubMod.Utility;

internal static class LoadEasyPrefabs
{
    public static void LoadPrefabs(AssetBundle assetBundle)
    {
        foreach (var easyPrefab in assetBundle.LoadAllAssets<EasyPrefab>())
        {
            PrefabInfo info = PrefabInfo.WithTechType(easyPrefab.techType.techTypeName, easyPrefab.unlockAtStart)
                .WithIcon(easyPrefab.sprite);

            var prefab = new CustomPrefab(info);
            if (easyPrefab.applySNShaders)
            {
                MaterialUtils.ApplySNShaders(easyPrefab.prefab);
            }

            if (easyPrefab.prefab != null)
            {
                prefab.SetGameObject(easyPrefab.prefab);
            }

            prefab.SetRecipeFromJson(Path.Combine(Plugin.RecipesFolderPath, $"{easyPrefab.techType.techTypeName}.json"));

            if (easyPrefab.unlockAtStart)
            {
                prefab.SetUnlock(TechType.None);
            }

            prefab.Register();
        }
    }
}
