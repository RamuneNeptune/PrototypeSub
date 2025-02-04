using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Interceptor;

internal class InterceptorReactorSequenceManager : MonoBehaviour
{
    [SerializeField] private MultipurposeAlienTerminal activationTerminal;
    [SerializeField] private InterfloorTeleporter teleporter;
    [SerializeField] private Transform returnPos;
    [SerializeField] private Vector3 islandTeleportPos;

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
    }

    public void EndReactorSequence()
    {
        IngameMenu_Patches.SetDenySaving(false);
        teleporter.StartTeleportPlayer(returnPos.position, returnPos.forward);
        Plugin.GlobalSaveData.reactorSequenceComplete = true;
        LargeWorldStreamer_Patches.SetOverwriteCamPos(false, Vector3.zero);
    }

    private void OnDestroy()
    {
        IngameMenu_Patches.SetDenySaving(false);
    }
}
