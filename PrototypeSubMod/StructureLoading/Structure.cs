using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nautilus.Handlers;
using Newtonsoft.Json;
using UnityEngine;

namespace PrototypeSubMod.StructureLoading;

public class Structure
{
    public Entity[] Entities { get; private set; }
    
    public bool IsSorted { get; private set; }
    
    public Structure(Entity[] entities)
    {
        Entities = entities;
    }
    
    public static Structure LoadFromFile(string jsonFilePath)
    {
        return JsonConvert.DeserializeObject<Structure>(File.ReadAllText(jsonFilePath));
    }
    
    public static Structure LoadFromBundle(string fileName)
    {
        return JsonConvert.DeserializeObject<Structure>(Plugin.AssetBundle.LoadAsset<TextAsset>(fileName).text);
    }
    
    public void SortByPriority()
    {
        Entities = Entities.OrderBy(( entity) => entity.priority).ToArray();
        IsSorted = true;
    }
}

public static class StructureExtensions
{
    private static List<string> registeredIds = new();
    
    public static void RegisterStructure(this Structure structure)
    {
        structure.SortByPriority();
        foreach (var entity in structure.Entities)
        {
            if (string.IsNullOrEmpty(entity.classId)) continue;
            
            if (registeredIds.Contains(entity.id)) continue;
            
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(entity.classId, entity.position.ToVector3(),
                entity.rotation.ToQuaternion(), entity.scale.ToVector3(),
                (obj) =>
                {
                    obj.GetComponent<UniqueIdentifier>().Id = entity.id;
                }));

            registeredIds.Add(entity.id);
        }
    }
}