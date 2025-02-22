using PrototypeSubMod.Compatibility;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Interceptor;

internal class InterceptorReactorSequenceManager : MonoBehaviour
{
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
            activationTerminal.onTerminalInteracted += StartReactorSequence;
        }
    }

    public void StartReactorSequence()
    {
        IngameMenu_Patches.SetDenySaving(true);
        teleporter.StartTeleportPlayer(islandTeleportPos, Camera.main.transform.forward);
        LargeWorldStreamer_Patches.SetOverwriteCamPos(true, transform.position);
        InterceptorIslandManager.Instance.OnTeleportToIsland(voidTeleportPos, this);
        InterceptorIslandManager.Instance.UpdateSeaglideLights(true);

        WeatherCompatManager.SetWeatherEnabled(false);
        WeatherCompatManager.SetWeatherClear();
    }

    public void EndReactorSequence()
    {
        IngameMenu_Patches.SetDenySaving(false);
        teleporter.StartTeleportPlayer(returnPos.position, returnPos.forward);
        Plugin.GlobalSaveData.reactorSequenceComplete = true;
        LargeWorldStreamer_Patches.SetOverwriteCamPos(false, Vector3.zero);
        GUIController_Patches.SetDenyHideCycling(false);
        GUIController.SetHidePhase(GUIController.HidePhase.None);
        WeatherCompatManager.SetWeatherEnabled(true);

        Player_Patches.SetOxygenReqOverride(false, 0);
    }

    public void OnTeleportToVoid()
    {
        InterceptorIslandManager.Instance.UpdateSeaglideLights(false);
        StartCoroutine(TeleportBackAfterDuration());
        Player_Patches.SetOxygenReqOverride(true, 0);
    }

    private IEnumerator TeleportBackAfterDuration()
    {
        yield return new WaitForSeconds(20f);

        EndReactorSequence();
    }

    private void OnDestroy()
    {
        IngameMenu_Patches.SetDenySaving(false);
        Player_Patches.SetOxygenReqOverride(false, 0);
    }
}
