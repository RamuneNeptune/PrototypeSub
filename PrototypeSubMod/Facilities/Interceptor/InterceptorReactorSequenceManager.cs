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
        IngameMenu_Patches.SetAllowSavingOverride(false);
        teleporter.StartTeleportPlayer(islandTeleportPos, Camera.main.transform.forward);
    }

    public void EndReactorSequence()
    {
        IngameMenu_Patches.SetAllowSavingOverride(false);
        teleporter.StartTeleportPlayer(returnPos.position, returnPos.forward);
        Plugin.GlobalSaveData.reactorSequenceComplete = true;
    }
}
