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
    }

    public void EndReactorSequence()
    {
        IngameMenu_Patches.SetDenySaving(false);
        teleporter.StartTeleportPlayer(returnPos.position, returnPos.forward);
        Plugin.GlobalSaveData.reactorSequenceComplete = true;
        LargeWorldStreamer_Patches.SetOverwriteCamPos(false, Vector3.zero);
    }

    public void OnTeleportToVoid()
    {
        StartCoroutine(TeleportBackAfterDuration());
    }

    private IEnumerator TeleportBackAfterDuration()
    {
        yield return new WaitForSeconds(20f);

        EndReactorSequence();
    }

    private void OnDestroy()
    {
        IngameMenu_Patches.SetDenySaving(false);
    }
}
