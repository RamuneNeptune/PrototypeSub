using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using PrototypeSubMod.Compatibility;
using System.IO;
using UnityEngine;

namespace PrototypeSubMod.Utility;

internal static class LoadEasyPrefabs
{
    public static void LoadPrefabs(AssetBundle assetBundle)
    {
        foreach (var easyPrefab in assetBundle.LoadAllAssets<EasyPrefab>())
        {
            PrefabInfo info = PrefabInfo.WithTechType(easyPrefab.techType.techTypeName, easyPrefab.unlockAtStart);
            if (easyPrefab.sprite != null)
            {
                info = info.WithIcon(easyPrefab.sprite);
            }

            var prefab = new CustomPrefab(info);
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

            if (easyPrefab.prefab != null)
            {
                prefab.SetGameObject(easyPrefab.prefab);
            }

            string path = Path.Combine(easyPrefab.jsonRecipePath, $"{easyPrefab.techType.techTypeName}.json");
            prefab.SetRecipe(ROTACompatManager.GetRelevantRecipe(path));

            if (easyPrefab.unlockAtStart)
            {
                prefab.SetUnlock(TechType.None);
            }
            else
            {
                prefab.SetUnlock(TechType.PrecursorKey_White);
            }

            prefab.Register();
        }
    }
}
