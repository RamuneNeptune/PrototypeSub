using System.Collections;
using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using UnityEngine;

namespace PrototypeSubMod.Prefabs.AlienBuildingBlock;

internal class ActiveAlienBuildingBlock : AlienBuildingBlock
{
    public static PrefabInfo prefabInfo { get; private set; }

    private static CustomPrefab prefab;
    
    public static void Register()
    {
        prefab = new CustomPrefab(PrefabInfo.WithTechType("ActiveAlienBuildingBlock", null, null, unlockAtStart: true).WithIcon(Plugin.AssetBundle.LoadAsset<Sprite>("AlienBuildingBlockIcon.png")));
        
        prefab.SetGameObject(GetPrefab);
        
        UWE.Utils.TryParseEnum<TechType>("InactiveAlienBuildingBlock", out var inactiveBlockTechType);
        prefab.SetRecipe(new RecipeData
        {
            craftAmount = 1,
            Ingredients = new List<CraftData.Ingredient>
            {
                new(inactiveBlockTechType)
            }
        }).WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Personal", "Equipment").WithCraftingTime(3f);
        
        prefab.Register();
    }
    
    private static IEnumerator GetPrefab(IOut<GameObject> prefab)
    {
        var returnPrefab = Plugin.AssetBundle.LoadAsset<GameObject>("ActiveAlienBuildingBlock.prefab");
        
        if(returnPrefab == null)
            Plugin.Logger.LogError("Failed to load the ActiveAlienBuildingBlock prefab.");

        returnPrefab.GetComponent<TechTag>().type = prefabInfo.TechType;
        returnPrefab.SetActive(false);

        var instance = Object.Instantiate(returnPrefab);
        
        var rootRelic = new TaskResult<GameObject>();
        yield return GetAlienBuildingBlockModel(rootRelic);

        var relicInstance = Object.Instantiate(rootRelic.Get(), instance.transform.GetChild(0));

        var relicMat = relicInstance.GetComponent<MeshRenderer>().materials[0];
        
        relicMat.SetFloat(ShaderPropertyID._GlowStrength, 20f);
        relicMat.SetFloat(ShaderPropertyID._GlowStrengthNight, 15f);
        
        prefab.Set(instance);
    }
}