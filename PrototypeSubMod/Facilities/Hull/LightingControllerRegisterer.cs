using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class LightingControllerRegisterer : MonoBehaviour
{
    [SerializeField] private LightingController lightingController;
    [SerializeField] private SkyApplier skyApplier;
    [SerializeField] private int defaultState;

    private void Start()
    {
        lightingController.RegisterSkyApplier(skyApplier);
        lightingController.SnapToState(defaultState);
    }
}