using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using UnityEngine;

namespace PrototypeSubMod.Prefabs;

internal class PrecursorIonCrystal_Craftable
{

    public static PrefabInfo craftableCrystalInfo;
    
    public static void Register()
    {
        string classID = "38ebd2e5-9dcc-4d7a-ada4-86a22e01191a";
        string filePath = "WorldEntities/EnvironmentResources/PrecursorIonCrystal.prefab";

        var prefabInfo = new PrefabInfo(classID, filePath, TechType.PrecursorIonCrystal);

        var crystal = new CustomPrefab(prefabInfo);
        
        var patch = new CustomPrefab("Proto_CrystalPlaceholder", "", "");

        craftableCrystalInfo = patch.Info;

        var template = new CloneTemplate(patch.Info, TechType.PrecursorIonCrystal)
        {
            ModifyPrefab = prefab =>
            {
                var vfxFabricating = prefab.transform.GetChild(0).gameObject.AddComponent<VFXFabricating>();

                vfxFabricating.localMinY = -0.4f;
                vfxFabricating.localMaxY = 0.4f;
                vfxFabricating.posOffset = new Vector3(0f, -0.05f, -0.05f);
                vfxFabricating.scaleFactor = 0.9f;
            }
        };
        
        patch.SetGameObject(template);

        var recipeData = new RecipeData
        {
            Ingredients = new CraftData.Ingredients
            {
                TechType.Titanium
            },
            craftAmount = 1
        };
        patch.AddGadget(new ScanningGadget(crystal, TechType.None)
            .WithPdaGroupCategory(Plugin.ProtoFabricatorGroup, Plugin.ProtoFabricatorCatgeory));
        patch.AddGadget(new CraftingGadget(crystal, recipeData).WithCraftingTime(3f));
        
        CraftData.pickupSoundList.Add(TechType.PrecursorIonCrystal, "event:/loot/pickup_precursorioncrystal");

        patch.Register();
    }
}