using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

[RequireComponent(typeof(Collider))]
internal class EntryWaterGate : MonoBehaviour
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private bool setInSub;

    private void OnTriggerEnter(Collider col)
    {
        var player = UWE.Utils.GetComponentInHierarchy<Player>(col.gameObject);

        if (!player || player.currChair != null) return;

        player.SetCurrentSub(setInSub ? subRoot : null, true);
    }
}
