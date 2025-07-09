using System;
using Newtonsoft.Json;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.StructureLoading;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal static class StructureRegisterer
{
    public static void Register()
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        Structure.LoadFromBundle("DefenseChamber").RegisterStructure();
        Structure.LoadFromBundle("DefenseTunnel").RegisterStructure();
        Structure.LoadFromBundle("EngineFacilityObjects").RegisterStructure();
        Structure.LoadFromBundle("EngineFacilityAdditions_Octo").RegisterStructure();
        Structure.LoadFromBundle("EngineFacilityExteriorObjects").RegisterStructure();
        Structure.LoadFromBundle("DefenseMoonpool").RegisterStructure();
        Structure.LoadFromBundle("ProtoItemDisplayCases").RegisterStructure();
        Structure.LoadFromBundle("ProtoIslands").RegisterStructure();
        Structure.LoadFromBundle("ProtoWarpCore").RegisterStructure();
        Structure.LoadFromBundle("DefenseFacilityDebris").RegisterStructure();
        Structure.LoadFromBundle("HullFacilityOutpost").RegisterStructure();
        Structure.LoadFromBundle("PrecursorFabricators").RegisterStructure();
        Structure.LoadFromBundle("HullFacilityObjects").RegisterStructure();
        Structure.LoadFromBundle("HullFacilityTunnelExtras").RegisterStructure();
        Structure.LoadFromBundle("HullFacilityKeyResources").RegisterStructure();
        Structure.LoadFromBundle("ProtoHullCave").RegisterStructure();
        Structure.LoadFromBundle("DefenseFacilityTeleporterRoomExteriorObjects").RegisterStructure();
        Structure.LoadFromBundle("DefenseFacilityTeleporterRoomInteriorObjects").RegisterStructure();
        Structure.LoadFromBundle("DefenseFacilityTeleporterRoom").RegisterStructure();
        
        if (TRPCompatManager.TRPInstalled)
        {
            Structure.LoadFromBundle("RedPlagueProtoIslands").RegisterStructure();
        }

        sw.Stop();
        Plugin.Logger.LogInfo($"Structures loaded in {sw.ElapsedMilliseconds}ms");
    }
}
