using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class LightingControllerRegisterer : MonoBehaviour
{
    [SerializeField] private LightingController lightingController;
    [SerializeField] private SkyApplier skyApplier;

    private void Start()
    {
        lightingController.RegisterSkyApplier(skyApplier);
        lightingController.SnapToState(0);
    }
}