using Nautilus.Extensions;
using PrototypeSubMod.Compatibility;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Interceptor;

internal class InterceptorReactorSequenceManager : MonoBehaviour
{
    private static readonly Vector3 VoidTeleportPos = new Vector3(-1590, -562, -288);

    private static InterfloorTeleporter Teleporter;
    private static Vector3 MostRecentReturnPos;

    [SerializeField] private MultipurposeAlienTerminal activationTerminal;
    [SerializeField] private InterfloorTeleporter teleporter;
    [SerializeField] private Transform returnPos;
    [SerializeField] private Vector3 islandTeleportPos;
    [SerializeField] private Vector3 voidTeleportPos;

    private void Start()
    {
        if (Plugin.GlobalSaveData.reactorSequenceComplete)
        {
            activationTerminal.ForceInteracted();
        }
        else
        {
            activationTerminal.onTerminalInteracted += () =>
            {
                MostRecentReturnPos = returnPos.position;
                StartReactorSequence();
            };
        }

        if (!Teleporter)
        {
            var teleporterHolder = new GameObject("IslandTeleporterHolder");
            Teleporter = teleporterHolder.AddComponent<InterfloorTeleporter>().CopyComponent(teleporter);
        }
    }

    public static void StartReactorSequence()
    {
        IngameMenu_Patches.SetDenySaving(true);
        BiomeGoalTracker_Patches.SetTrackingBlocked(true);

        Teleporter.StartTeleportPlayer(InterceptorIslandManager.Instance.GetRespawnPoint(), Camera.main.transform.forward);
        LargeWorldStreamer_Patches.SetOverwriteCamPos(true, Camera.main.transform.position);
        InterceptorIslandManager.Instance.OnTeleportToIsland(VoidTeleportPos);
        InterceptorIslandManager.Instance.UpdateSeaglideLights(true);

        WeatherCompatManager.SetWeatherEnabled(false);
        WeatherCompatManager.SetWeatherClear();
    }

    public static void EndReactorSequence()
    {
        IngameMenu_Patches.SetDenySaving(false);
        Teleporter.StartTeleportPlayer(MostRecentReturnPos, Camera.main.transform.forward);
        Plugin.GlobalSaveData.reactorSequenceComplete = true;
        LargeWorldStreamer_Patches.SetOverwriteCamPos(false, Vector3.zero);
        GUIController_Patches.SetDenyHideCycling(false);
        GUIController.SetHidePhase(GUIController.HidePhase.None);
        WeatherCompatManager.SetWeatherEnabled(true);

        Player_Patches.SetOxygenReqOverride(false, 0);
        BiomeGoalTracker_Patches.SetTrackingBlocked(false);
    }

    public static void OnTeleportToVoid()
    {
        InterceptorIslandManager.Instance.UpdateSeaglideLights(false);
        UWE.CoroutineHost.StartCoroutine(TeleportBackAfterDuration());
        Player_Patches.SetOxygenReqOverride(true, 0);
    }

    private static IEnumerator TeleportBackAfterDuration()
    {
        yield return new WaitForSeconds(20f);

        EndReactorSequence();
    }

    private void OnDestroy()
    {
        IngameMenu_Patches.SetDenySaving(false);
        Player_Patches.SetOxygenReqOverride(false, 0);
        LargeWorldStreamer_Patches.SetOverwriteCamPos(false, Vector3.zero);
        BiomeGoalTracker_Patches.SetTrackingBlocked(false);
    }
}
