using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class TeleporterDisablerBehavior : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitUntil(LargeWorldStreamer.main.IsWorldSettled);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
        foreach (var collider in colliders)
        {
            var teleporter = collider.GetComponentInParent<PrecursorTeleporter>();
            if (!teleporter) continue;

            teleporter.teleporterIdentifier = "teasertp";
            teleporter.ToggleDoor(false);
            break;
        }
    }
}
