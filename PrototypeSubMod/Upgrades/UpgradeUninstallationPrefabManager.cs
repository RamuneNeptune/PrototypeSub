using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using Nautilus.Utility;
using PrototypeSubMod.SubTerminal;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal static class UpgradeUninstallationPrefabManager
{
    private static bool Initialized;

    public static void RegisterUninstallationPrefabs(AssetBundle assetBundle)
    {
        if (Initialized) return;

        var uninstallationTechTypes = assetBundle.LoadAllAssets<UninstallationTechType>();

        foreach (var techType in uninstallationTechTypes)
        {
            TechType originalTechType = techType.ownerType.TechType;
            PrefabInfo info = PrefabInfo.WithTechType($"{originalTechType}{techType.nameSuffix}")
                .WithIcon(techType.sprite);

            var prefab = new CustomPrefab(info);
            List<TechType> ingredients = new();

            foreach (var ingredient in CraftDataHandler.GetRecipeData(originalTechType).Ingredients)
            {
                for (int i = 0; i < ingredient.amount; i++)
                {
                    ingredients.Add(ingredient.techType);
                }
            }

            var data = new Nautilus.Crafting.RecipeData()
            {
                craftAmount = 0,
                Ingredients = new(),
                LinkedItems = ingredients
            };

            CraftDataHandler.SetRecipeData(info.TechType, data);
            prefab.SetUnlock(originalTechType);
            prefab.SetGameObject(GetGameObject(info.ClassID, info.TechType));

            prefab.Register();

            uGUI_ProtoUpgradeIcon.SetUninstallationTechType(originalTechType, info.TechType);
        }

        Initialized = true;
    }

    private static GameObject GetGameObject(string classID, TechType techType)
    {
        var empty = Plugin.AssetBundle.LoadAsset<GameObject>("Empty");
        empty.SetActive(false);
        var instance = GameObject.Instantiate(empty);
        PrefabUtils.AddBasicComponents(instance, classID, techType, LargeWorldEntity.CellLevel.Near);
        GameObject.DestroyImmediate(instance.GetComponent<SkyApplier>());

        return instance;
    }
}
