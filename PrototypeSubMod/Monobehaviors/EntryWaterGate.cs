using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

[RequireComponent(typeof(Collider))]
internal class EntryWaterGate : MonoBehaviour
{
    [SerializeField] private bool setWalk;
    [SerializeField] private SubRoot subRoot;

    private void OnTriggerEnter(Collider col)
    {
        var player = UWE.Utils.GetComponentInHierarchy<Player>(col.gameObject);

        if (!player || player.currChair != null) return;

        if (!subRoot)
        {
            player.SetPrecursorOutOfWater(setWalk);
            return;
        }

        player.SetCurrentSub(setWalk ? subRoot : null, true);
    }
}
