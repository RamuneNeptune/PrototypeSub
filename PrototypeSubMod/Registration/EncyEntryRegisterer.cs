using Nautilus.Handlers;
using PrototypeSubMod.Prefabs;
using System.Collections.Generic;
using PrototypeSubMod.Prefabs.FacilityProps.Hull;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal static class EncyEntryRegisterer
{
    public static void Register()
    {
        #region Prototype
        string protoTitle = Language.main.Get("ProtoDatabankEncy_Title");
        string protoBody = Language.main.Get("ProtoDatabankEncy_Body");
        Texture2D prototypeBackground = Plugin.AssetBundle.LoadAsset<Texture2D>("PrototypeSubEncy");

        PDAHandler.AddEncyclopediaEntry("ProtoDatabankEncy", "DownloadedData/Precursor/Terminal", protoTitle, protoBody,
            image: prototypeBackground, unlockSound: PDAHandler.UnlockImportant);
        #endregion

        #region Precursor Ingot
        string ingotTitle = Language.main.Get("ProtoPrecursorIngotEncy_Title");
        string ingotDescription = Language.main.Get("ProtoPrecursorIngotEncy_Body");
        var ingotPopup = Plugin.AssetBundle.LoadAsset<Sprite>("AlienFramework_EncyPopup");

        PDAHandler.AddEncyclopediaEntry("ProtoPrecursorIngot", "DownloadedData/Precursor/Scan", ingotTitle,
            ingotDescription, unlockSound: PDAHandler.UnlockBasic, popupImage: ingotPopup);
        var precursorIngotEntryData = new PDAScanner.EntryData()
        {
            key = PrecursorIngot_Craftable.prefabInfo.TechType,
            destroyAfterScan = false,
            encyclopedia = "ProtoPrecursorIngot",
            scanTime = 5f,
            isFragment = false,
            blueprint = PrecursorIngot_Craftable.prefabInfo.TechType
        };
        PDAHandler.AddCustomScannerEntry(precursorIngotEntryData);
        #endregion

        #region Deployable Light
        string lightTitle = Language.main.Get("ProtoDeployableLightEncy_Title");
        string lightDescription = Language.main.Get("ProtoDeployableLightEncy_Body");
        var lightPopup = Plugin.AssetBundle.LoadAsset<Sprite>("DeployableLight_EncyPopup");
        Texture2D lightBackground = Plugin.AssetBundle.LoadAsset<Texture2D>("PhotonBeaconEncy");
        
        PDAHandler.AddEncyclopediaEntry("ProtoDeployableLightEncy", "Tech/Equipment", lightTitle, lightDescription, image: lightBackground, unlockSound: PDAHandler.UnlockBasic, popupImage: lightPopup);
        var deployableLightEntryData = new PDAScanner.EntryData()
        {
            key = DeployableLight_Craftable.prefabInfo.TechType,
            destroyAfterScan = false,
            encyclopedia = "ProtoDeployableLightEncy",
            scanTime = 5f,
            isFragment = false,
            blueprint = DeployableLight_Craftable.prefabInfo.TechType
        };
        PDAHandler.AddCustomScannerEntry(deployableLightEntryData);
        #endregion

        #region Ion Prism
        string prismTitle = Language.main.Get("ProtoIonPrismEncy_Title");
        string prismDescription = Language.main.Get("ProtoIonPrismEncy_Body");
        var prismPopup = Plugin.AssetBundle.LoadAsset<Sprite>("IonPrism_EncyPopup");

        PDAHandler.AddEncyclopediaEntry("ProtoIonPrismEncy", "DownloadedData/Precursor/Scan", prismTitle,
            prismDescription, unlockSound: PDAHandler.UnlockBasic, popupImage: prismPopup);
        var ionPrismEntryData = new PDAScanner.EntryData()
        {
            key = IonPrism_Craftable.prefabInfo.TechType,
            destroyAfterScan = false,
            encyclopedia = "ProtoIonPrismEncy",
            scanTime = 5f,
            isFragment = false,
            blueprint = IonPrism_Craftable.prefabInfo.TechType
        };
        PDAHandler.AddCustomScannerEntry(ionPrismEntryData);
        #endregion

        #region Orion Logs
        string orionTitle = Language.main.Get("OrionFacilityLogs_Title");
        string orionBody = Language.main.Get("OrionFacilityLogs_Body");
        PDAHandler.AddEncyclopediaEntry("OrionFacilityLogsEncy", "DownloadedData/Precursor/Terminal", orionTitle,
            orionBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Defense Audit Logs
        string defenseAuditTitle = Language.main.Get("DefenseFacilityLogs_Title");
        string defenseAuditBody = Language.main.Get("DefenseFacilityLogs_Body");
        PDAHandler.AddEncyclopediaEntry("DefenseFacilityAuditEncy", "DownloadedData/Precursor/Terminal",
            defenseAuditTitle, defenseAuditBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Engine Audit Logs
        string engineAuditTitle = Language.main.Get("EngineFacilityLogs_Title");
        string engineAuditBody = Language.main.Get("EngineFacilityLogs_Body");
        PDAHandler.AddEncyclopediaEntry("EngineFacilityAuditEncy", "DownloadedData/Precursor/Terminal",
            engineAuditTitle, engineAuditBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Facility Locations
        string locationsTitle = Language.main.Get("ProtoFacilitiesEncy_Title");
        string locationsBody = Language.main.Get("ProtoFacilitiesEncy_Body");
        PDAHandler.AddEncyclopediaEntry("ProtoFacilitiesEncy", "DownloadedData/Precursor/Terminal", locationsTitle,
            locationsBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Orion Fragmentor
        string fragmentorTitle = Language.main.Get("OrionFragmentorEncy_Title");
        string fragmentorBody = Language.main.Get("OrionFragmentorEncy_Body");
        PDAHandler.AddEncyclopediaEntry("OrionFragmentorEncy", "DownloadedData/Precursor/Scan", fragmentorTitle,
            fragmentorBody, unlockSound: PDAHandler.UnlockBasic);
        
        var orionFragmentorEntryData = new PDAScanner.EntryData()
        {
            key = OrionFragmentor_World.prefabInfo.TechType,
            destroyAfterScan = false,
            encyclopedia = "OrionFragmentorEncy",
            scanTime = 10f,
            isFragment = false,
            blueprint = OrionFragmentor_World.prefabInfo.TechType
        };
        PDAHandler.AddCustomScannerEntry(orionFragmentorEntryData);
        #endregion
        
        #region Hull Facility Logs
        string hullFacilityLogsTitle = Language.main.Get("HullFacilityLogsEncy_Title");
        string hullFacilityLogsBody = Language.main.Get("HullFacilityLogsEncy_Body");
        PDAHandler.AddEncyclopediaEntry("HullFacilityLogsEncy", "DownloadedData/Precursor/Terminal",
            hullFacilityLogsTitle, hullFacilityLogsBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Orion Endeavors
        string orionEndeavorsTitle = Language.main.Get("OrionEndeavorsEncy_Title");
        string orionEndeavorsBody = Language.main.Get("OrionEndeavorsEncy_Body");
        PDAHandler.AddEncyclopediaEntry("OrionEndeavorsEncy", "DownloadedData/Precursor/Terminal", orionEndeavorsTitle,
            orionEndeavorsBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Alien Building Block
        string alienBuildingBlockEncyTitle = Language.main.Get("AlienBuildingBlockEncy_Title");
        string alienBuildingBlockEncyBody = Language.main.Get("AlienBuildingBlockEncy_Body");
        PDAHandler.AddEncyclopediaEntry("AlienBuildingBlockEncy", "DownloadedData/Precursor/Terminal",
            alienBuildingBlockEncyTitle, alienBuildingBlockEncyBody, unlockSound: PDAHandler.UnlockBasic);
        #endregion

        #region Hull Facility Tablet
        string hullTabletTitle = Language.main.Get("HullFacilityTabletEncy_Title");
        string hullTabletDescription = Language.main.Get("HullFacilityTabletEncy_Body");
        var hullTabletPopup = Plugin.AssetBundle.LoadAsset<Sprite>("HullFacilityTablet_EncyPopup");
        
        PDAHandler.AddEncyclopediaEntry("HullFacilityTabletEncy", "DownloadedData/Precursor/Scan", hullTabletTitle, hullTabletDescription, unlockSound: PDAHandler.UnlockBasic, popupImage: hullTabletPopup);
        #endregion
        
        RegisterEncyEntries("DownloadedData/Precursor/ProtoUpgrades", PDAHandler.UnlockBasic, new()
            {
                "ProtoCloakEncy",
                "ProtoEmergencyWarpEncy",
                "ProtoInterceptorEncy",
                "ProtoRepairDroidsEncy",
                "ProtoPressureConvertersEncy",
                "ProtoIonBarrierEncy",
                "ProtoStasisPulseEncy",
                "ProtoVentilatorsEncy",
                "ProtoIonGeneratorEncy",
                "ProtoOverclockEncy"
            });
    }

    private static void RegisterEncyEntries(string path, FMODAsset unlockSound, List<string> entries)
    {
        foreach (var entry in entries)
        {
            string title = Language.main.Get($"{entry}_Title");
            string body = Language.main.Get($"{entry}_Body");
            PDAHandler.AddEncyclopediaEntry(entry, path, title, body, unlockSound: unlockSound);
        }
    }
}
