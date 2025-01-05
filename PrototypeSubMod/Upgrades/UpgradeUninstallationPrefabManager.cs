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

        var buildTerminal = assetBundle.LoadAsset<GameObject>("ProtoBuildTerminal");

        foreach (var upgrade in buildTerminal.GetComponentsInChildren<uGUI_ProtoUpgradeIcon>(true))
        {
            TechType originalTechType = upgrade.GetUpgradeTechType();
            PrefabInfo info = PrefabInfo.WithTechType($"{originalTechType}_Uninstalled")
                .WithIcon(upgrade.uninstallSprite);

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

            upgrade.SetUninstallationTechType(info.TechType);
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
