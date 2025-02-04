using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Patches;
using PrototypeSubMod.Utility;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Interceptor;

internal class InterceptorIslandManager : MonoBehaviour
{
    public static InterceptorIslandManager Instance { get; private set; }

    [SerializeField] private MultipurposeAlienTerminal terminal;
    [SerializeField] private InterfloorTeleporter teleporter;
    [SerializeField] private GameObject islandObjects;
    [SerializeField] private DummyTechType emergencyWarpTechType;
    [SerializeField] private string emergencyWarpEncyKey;

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
        StartCoroutine(TeleportPlayerDelayed());
    }

    private IEnumerator TeleportPlayerDelayed()
    {
        KnownTech.Add(emergencyWarpTechType.TechType);
        PDAEncyclopedia.Add(emergencyWarpEncyKey, true);

        yield return new WaitForSeconds(10f);

        teleporter.StartTeleportPlayer(voidTeleportPos, Camera.main.transform.forward);
        sequenceManager.OnTeleportToVoid();
        GUIController.SetHidePhase(GUIController.HidePhase.HUD);
        GUIController_Patches.SetDenyHideCycling(true);

        yield return new WaitForSeconds(1f);

        SetIslandEnabled(false);
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
