using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Interceptor;

internal class InterceptorIslandManager : MonoBehaviour
{
    public static InterceptorIslandManager Instance { get; private set; }

    [SerializeField] private MultipurposeAlienTerminal terminal;
    [SerializeField] private InterfloorTeleporter teleporter;
    [SerializeField] private GameObject islandObjects;

    private Vector3 voidTeleportPos;
    private InterceptorReactorSequenceManager sequenceManager;

    private void Awake()
    {
        if (Instance != null) throw new System.Exception("More than one InterceptorIslandManager in the scene. How did this even happen?");

        Instance = this;
    }

    private void Start()
    {
        terminal.onTerminalInteracted += OnTerminalInteracted;
        SetIslandEnabled(false);
    }

    private void OnTerminalInteracted()
    {
        teleporter.StartTeleportPlayer(voidTeleportPos, Camera.main.transform.forward);
        sequenceManager.OnTeleportToVoid();
        SetIslandEnabled(false);
        GUIController.SetHidePhase(GUIController.HidePhase.HUD);
        GUIController_Patches.SetDenyHideCycling(true);
    }

    public void SetIslandEnabled(bool enabled)
    {
        islandObjects.SetActive(enabled);
    }

    public void OnTeleportToIsland(Vector3 voidPosition, InterceptorReactorSequenceManager sequenceManager)
    {
        voidTeleportPos = voidPosition;
        this.sequenceManager = sequenceManager;
        SetIslandEnabled(true);
    }
}
