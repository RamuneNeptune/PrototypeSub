using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PrototypeSubMod.Utility;

public class ProtoMatDatabase : ProtoMatDatabaseBase
{
    public static int matDatabaseSize
    {
        get
        {
            if (matDatabase == null)
                return -1;

            return matDatabase.Count;
        }
    }
    
    public static void Initalize()
    {
        BuildMaterialDatabase();
    }

    public static IEnumerator ReplaceVanillaMats(GameObject customObject)
    {
        if (customObject == null)
        {
            Plugin.Logger.LogError("A null object was passed into the ReplaceVanillaMats method!");
            yield break;
        }
        
        var renderers = customObject.GetAllComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Plugin.Logger.LogError($"Object {customObject.name} does not have any renderers.");
            yield break;
        }

        List<Material> replaceMaterials = new();
        List<string> skipMaterialNames = new();
        foreach (var renderer in renderers)
        {
            var newMatList = renderer.materials;
            
            for(int i = 0; i < newMatList.Length; i++)
            {
                var matName = RemoveInstanceFromMatName(newMatList[i].name);

                bool skipMaterial = skipMaterialNames.Contains(matName);

                if (!skipMaterial)
                {
                    foreach (var mat in replaceMaterials)
                    {
                        if (mat.name.Equals(matName))
                        {
                            newMatList[i] = mat;
                            skipMaterial = true;
                            break;
                        }
                    }
                }

                if (skipMaterial)
                    continue;
                
                var taskResult = new TaskResult<Material>();
                
                yield return TryGetMatFromDatabase(RemoveInstanceFromMatName(renderer.materials[i].name), taskResult);

                var foundMaterial = taskResult.value;

                if (foundMaterial == null)
                    continue;
                
                newMatList[i] = foundMaterial;
                replaceMaterials.Add(foundMaterial);
            }
            
            renderer.materials = newMatList;
        }
    }

    public static IEnumerator TryGetMatFromDatabase(string matName, IOut<Material> materialResult)
    {
        Material matResult = null;

        if (matDatabase.TryGetValue(matName, out var matPath))
        {
            Material returnedMat;

            /*
            For whatever reason, even after the database is finished, and the entry for the material is found to
            exist, loading it through these means will occasionally result in a null material return value. We know
            that none of the materials are actually null, however. For this reason, we just re-attempt to load the material
            at the specified file path until it's fetched successfully. All testing indicates that this rarely happens,
            and when it does, only requires 1-4 retries max.
            */
            do
            {
                var handle = AddressablesUtility.LoadAsync<Material>(matPath);
                
                yield return handle.Task;

                returnedMat = handle.Result;
            } while (returnedMat == null);

            matResult = returnedMat;
        }
        else
            Plugin.Logger.LogDebug($"Failed to find material {matName} in Material Database.");
        
        materialResult.Set(matResult);
    }
}

public abstract class ProtoMatDatabaseBase
{
    protected static Dictionary<string, string> matDatabase = new();
    
    protected static void BuildMaterialDatabase()
    {
        matDatabase.Clear();
        
        RegisterProjectMats();
    }

    protected static string RemoveInstanceFromMatName(string originalMatName)
    {
        string returnValue = originalMatName;
        
        if(originalMatName.EndsWith(" (Instance)"))
            returnValue = originalMatName.Substring(0, originalMatName.Length - " (Instance)".Length);
        
        return returnValue;
    }

    private static void RegisterProjectMats()
    {
        var file = new FileInfo(Path.Combine(Path.GetDirectoryName(Plugin.Assembly.Location), "MaterialDatabase") + "/MatFilePathMap.json");

        if (!file.Exists)
        {
            Plugin.Logger.LogError("Failed to get .json file for the MatDatabase!");
            return;
        }
        
        var streamReader = new StreamReader(file.FullName);
        
        string fileContents = streamReader.ReadToEnd();

        foreach (var entry in JObject.Parse(fileContents))
        {
            RegisterMat(entry.Key, entry.Value.ToString());
        }
    }
    
    private static void RegisterMat(string matName, string matPath)
    {
        //TODO: Note that final matDatabase size after project mats are obtained should be 1427.
        if (!matDatabase.ContainsKey(matName))
        {
            Plugin.Logger.LogDebug($"Added material #{matDatabase.Count}: {matName} to database.");
            matDatabase.Add(matName, matPath);
        }
    }
}