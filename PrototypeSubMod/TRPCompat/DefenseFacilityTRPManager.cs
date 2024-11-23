using PrototypeSubMod.Compatibility;
using UnityEngine;

namespace PrototypeSubMod.TRPCompat;

internal class DefenseFacilityTRPManager : MonoBehaviour
{
    [SerializeField] private GameObject trpIsland;
    [SerializeField] private Collider safetyTeleporterCutout;

    private void Start()
    {
        trpIsland.SetActive(TRPCompatManager.TRPInstalled);
        safetyTeleporterCutout.enabled = !TRPCompatManager.TRPInstalled;
    }
}
