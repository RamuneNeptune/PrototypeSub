using Nautilus.Handlers;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.PrototypeStory;
using UnityEngine;

namespace PrototypeSubMod.Registration;

internal class StoryGoalsRegisterer
{
    public static void Register()
    {
        #region Precursor Ingot

        StoryGoalHandler.RegisterItemGoal("Ency_ProtoPrecursorIngot", Story.GoalType.Encyclopedia, PrecursorIngot_Craftable.prefabInfo.TechType);

        StoryGoalHandler.RegisterCustomEvent("Ency_ProtoPrecursorIngot", () =>
        {
            KnownTech.Add(PrecursorIngot_Craftable.prefabInfo.TechType);
            PDAEncyclopedia.Add("ProtoPrecursorIngot", true);
        });

        #endregion

        #region PPT First Interaction
        PPTStoryManager.RegisterGoals();
        #endregion

        #region Interceptor Unlock
        StoryGoalHandler.RegisterCustomEvent("OnInterceptorTestDataDownloaded", () =>
        {
            PDALog.Add("OnInterceptorTestDataDownloaded");
        });

        StoryGoalHandler.RegisterCompoundGoal("InterceptorTestEncy", Story.GoalType.Encyclopedia, 15f, new[] { "OnInterceptorTestDataDownloaded" });
        StoryGoalHandler.RegisterCustomEvent("InterceptorTestEncy", () =>
        {
            PDAEncyclopedia.Add("InterceptorTestEncy", true);
        });
        #endregion

        #region Disable Defense Cloak
        StoryGoalHandler.RegisterCustomEvent("OnDefenseCloakDisabled", () =>
        {
            PDALog.Add("OnDefenseCloakDisabled");
        });
        #endregion

        #region Moonpool Enter
        StoryGoalHandler.RegisterCustomEvent("OnEnterDefenseMoonpool", () =>
        {
            PDALog.Add("OnEnterDefenseMoonpool");
        });

        StoryGoalHandler.RegisterLocationGoal("OnEnterDefenseMoonpool", Story.GoalType.PDA, new Vector3(819, -463, -1115), 15, 0);
        #endregion

        #region Moonpool Open Disallowed
        StoryGoalHandler.RegisterCustomEvent("OnMoonpoolNoPrototype", () =>
        {
            PDALog.Add("OnMoonpoolNoPrototype");
        });
        #endregion

        #region On Approach Defense Beacon
        StoryGoalHandler.RegisterCustomEvent("OnApproachDefenseFacility", () =>
        {
            PDALog.Add("OnApproachDefenseFacility");
        });
        #endregion

        #region Orion Logs
        StoryGoalHandler.RegisterCustomEvent("Ency_OrionFacilityLogs", () =>
        {
            PDAEncyclopedia.Add("OrionFacilityLogsEncy", true);
        });
        #endregion

        #region Facility Locations
        StoryGoalHandler.RegisterCustomEvent("Ency_ProtoFacilitiesEncy", () =>
        {
            PDAEncyclopedia.Add("ProtoFacilitiesEncy", true);
        });
        #endregion

        #region Defense Audit Logs
        StoryGoalHandler.RegisterCompoundGoal("DefenseFacilityAuditEncy", Story.GoalType.Encyclopedia, 7f, "OnDisableDefenseCloak");

        StoryGoalHandler.RegisterCustomEvent("DefenseFacilityAuditEncy", () =>
        {
            PDAEncyclopedia.Add("DefenseFacilityAuditEncy", true);
        });
        #endregion

        #region Engine Audit Logs
        StoryGoalHandler.RegisterCustomEvent("EngineFacilityAuditEncy", () =>
        {
            PDAEncyclopedia.Add("EngineFacilityAuditEncy", true);
        });
        #endregion
    }
}
