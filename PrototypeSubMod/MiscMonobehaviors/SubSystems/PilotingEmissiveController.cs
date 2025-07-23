using PrototypeSubMod.EngineLever;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class PilotingEmissiveController : MonoBehaviour
{
    [SerializeField] private LightingController controller;
    [SerializeField] private ProtoEngineLever engineLever;

    private void Start()
    {
        engineLever.onEngineStateChanged += OnEngineChanged;
        controller.emissiveController.RegisterRenderers(GetComponentsInChildren<Renderer>());
    }

    private void OnEngineChanged(bool engineOn)
    {
        controller.LerpToState(engineOn ? 1 : 0);
    }
}