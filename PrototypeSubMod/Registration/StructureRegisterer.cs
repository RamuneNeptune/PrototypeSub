using System;
using EpicStructureLoader;
using ModStructureFormat;
using Newtonsoft.Json;
using PrototypeSubMod.Compatibility;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal static class StructureRegisterer
{
    public static void Register()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        int entityCount = 0;
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseChamber"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseTunnel"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("EngineFacilityObjects"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("EngineFacilityAdditions_Octo"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("EngineFacilityExteriorObjects"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseMoonpool"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoItemDisplayCases"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoIslands"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("TempHullFacility"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoWarpCore"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseFacilityDebris"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("HullFacilityOutpost"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("PrecursorFabricators"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("HullFacilityObjects"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("HullFacilityTunnelExtras"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("HullFacilityKeyResources"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("ProtoHullCave"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseFacilityTeleporterRoomExteriorObjects"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseFacilityTeleporterRoomInteriorObjects"), ref entityCount);
        StructureLoading.RegisterStructure(LoadStructureFromBundle("DefenseFacilityTeleporterRoom"), ref entityCount);
        
        if (TRPCompatManager.TRPInstalled)
        {
            var trpIslandFile = Plugin.AssetBundle.LoadAsset<TextAsset>("RedPlagueProtoIslands");
            var trpIsland = JsonConvert.DeserializeObject<Structure>(trpIslandFile.text);

            StructureLoading.RegisterStructure(trpIsland, ref entityCount);
        }

        sw.Stop();
        Plugin.Logger.LogInfo($"Structures loaded in {sw.ElapsedMilliseconds}ms");
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
