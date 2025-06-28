using Nautilus.Crafting;
using Nautilus.Json.Converters;
using Newtonsoft.Json;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.PowerSystem.Funcionalities;
using System;
using System.Collections.Generic;
using System.IO;

namespace PrototypeSubMod.Compatibility;

internal static class ROTACompatManager
{
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
                _architectLibInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.aotu.architectslibrary");
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
        powerSources.Add(electricube, new PowerConfigData(5, typeof(ElectricubePowerFunctionality)));
        powerSources.Add(powerCube, new PowerConfigData(5, typeof(PowerCubeFunctionality)));
    }

    /// <summary>
    /// If AL is installed, returns the recipe containing AL items. If not or such recipe does not exist, returns the default recipe
    /// </summary>
    /// <param name="recipePath">The path to the recipe inside the recipe folder</param>
    /// <returns>The relevant recipe</returns>
    public static RecipeData GetRelevantRecipe(string recipePath)
    {
        string checkPath = Path.Combine(Plugin.RecipesFolderPath, "AL", recipePath);
        string normalPath = Path.Combine(Plugin.RecipesFolderPath, "Normal", recipePath);

        string ALPath = File.Exists(checkPath) ? checkPath : normalPath;
        string path = normalPath;

        string json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<RecipeData>(json, new CustomEnumConverter());
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
