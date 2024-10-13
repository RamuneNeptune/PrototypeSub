using Nautilus.Crafting;
using Nautilus.Json.Converters;
using Newtonsoft.Json;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Prefabs;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Must;
using Ingredient = CraftData.Ingredient;

namespace PrototypeSubMod.Compatibility;

internal static class ROTACompatManager
{
    // ~3 Proto ingots to one Architect one
    private const float INGOT_CONVERSION_RATE = 0.3f;

    public static bool RotAInstalled
    {
        get
        {
            if (!_rotaInitted)
            {
                _rotaInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ReturnOfTheAncients");
                _rotaInitted = true;
            }

            return _rotaInstalled;
        }
    }

    private static bool _rotaInitted;
    private static bool _rotaInstalled;

    public static bool ArchitectsLibInstalled
    {
        get
        {
            if (!_architectLibInitted)
            {
                _architectLibInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ArchitectsLibrary");
                _architectLibInitted = true;
            }

            return _architectLibInstalled;
        }
    }

    private static bool _architectLibInitted;
    private static bool _architectLibInstalled;

    public static void AddCompatiblePowerSources()
    {
        if (!ArchitectsLibInstalled) return;

        TechType electricube = (TechType)Enum.Parse(typeof(TechType), "Electricube");
        TechType powerCube = (TechType)Enum.Parse(typeof(TechType), "RedIonCube");

        var powerSources = PrototypePowerSystem.AllowedPowerSources;
        powerSources.Add(electricube, 2500);
        powerSources.Add(powerCube, 3000);
    }

    /// <summary>
    /// Corrects references to "PrecursorIngot" in a json recipe to either the ArchitectsLibrary precursor ingot or
    /// the Prototype Precursor Ingot, depending on if the library is installed
    /// </summary>
    /// <param name="jsonRecipeData">The json recipe data</param>
    /// <returns>The recipe data with the correct ingot type</returns>
    public static RecipeData SwapRecipeToCorrectIngot(string jsonRecipeData)
    {
        if (ArchitectsLibInstalled)
        {
            var data = JsonConvert.DeserializeObject<RecipeData>(jsonRecipeData, new CustomEnumConverter());
            foreach (var item in data.Ingredients)
            {
                if (item.techType.ToString().ToLower() != "precursoringot")
                {
                    continue;
                }

                item._amount = Mathf.FloorToInt(Mathf.Max(1, item.amount * INGOT_CONVERSION_RATE));
            }

            return data;
        }

        var dummyRecipeData = JsonConvert.DeserializeObject<DummyRecipeData>(jsonRecipeData);
        var recipeData = new RecipeData();

        recipeData.craftAmount = dummyRecipeData.craftAmount;
        recipeData.Ingredients = DummyToRealIngredients(dummyRecipeData.Ingredients);

        return recipeData;
    }

    /// <summary>
    /// If AL is installed, retunrs the recipe containing AL items. If not or such recipe does not exist, returns the default recipe
    /// </summary>
    /// <param name="recipePath">The path to the recipe inside the recipe folder</param>
    /// <returns>The relevant recipe</returns>
    public static RecipeData GetRelevantRecipe(string recipePath)
    {
        string checkPath = Path.Combine(Plugin.RecipesFolderPath, "AL", recipePath);
        string normalPath = Path.Combine(Plugin.RecipesFolderPath, "Normal", recipePath);

        string ALPath = File.Exists(checkPath) ? checkPath : normalPath;
        string path = ArchitectsLibInstalled ? ALPath : normalPath; 

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<RecipeData>(json, new CustomEnumConverter());
    }

    private static List<Ingredient> DummyToRealIngredients(List<DummyIngredient> ingredients)
    {
        List<Ingredient> newIngredients = new();
        foreach (var item in ingredients)
        {
            TechType type = TechType.None;
            if (item.techType.ToLower() == "precursoringot")
            {
                type = PrecursorIngot_Craftable.prefabInfo.TechType;
            }
            else
            {
                type = (TechType)Enum.Parse(typeof(TechType), item.techType);
            }

            newIngredients.Add(new Ingredient(type, item.amount));
        }

        return newIngredients;
    }

    private struct DummyRecipeData
    {
        public int craftAmount;
        public List<DummyIngredient> Ingredients;
        public List<string> LinkedItems;

        public DummyRecipeData(int craftAmount, List<DummyIngredient> Ingredients, List<string> LinkedItems = null)
        {
            this.craftAmount = craftAmount;
            this.Ingredients = Ingredients;
            this.LinkedItems = LinkedItems;
        }
    }

    private struct DummyIngredient
    {
        public string techType;
        public int amount;

        public DummyIngredient(string techType, int amount)
        {
            this.techType = techType;
            this.amount = amount;
        }
    }
}
