using System;
using EpicStructureLoader;
using ModStructureFormat;
using Newtonsoft.Json;
using PrototypeSubMod.Compatibility;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal class StructureRegisterer
{
    public static void Register()
    {
        int entityCount = 0;
        //StructureLoading.RegisterStructure(LoadStructureFromBundle("InterceptorFacility"), ref entityCount);
        //entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseChamber"), ref entityCount);
        entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseTunnel"), ref entityCount);
        entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("EngineFacility"), ref entityCount);
        entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseMoonpool"), ref entityCount);
        entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoItemDisplayCases"), ref entityCount);
        entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoIslands"), ref entityCount);
        entityCount = 0;

        StructureLoading.RegisterStructure(LoadStructureFromBundle("TempHullFacility"), ref entityCount);
        entityCount = 0;
        
        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoWarpCore"), ref entityCount);
        entityCount = 0;
        
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseFacilityDebris"), ref entityCount);
        entityCount = 0;

        if (TRPCompatManager.TRPInstalled)
        {
            var trpIslandFile = Plugin.AssetBundle.LoadAsset<TextAsset>("RedPlagueProtoIslands");
            var trpIsland = JsonConvert.DeserializeObject<Structure>(trpIslandFile.text);

            StructureLoading.RegisterStructure(trpIsland, ref entityCount);
        }
    }

    private static Structure LoadStructureFromBundle(string name)
    {
        var structureFile = Plugin.AssetBundle.LoadAsset<TextAsset>(name);
        if (!structureFile)
        {
            return new Structure(Array.Empty<Entity>());
        }

        return JsonConvert.DeserializeObject<Structure>(structureFile.text);
    }
}
