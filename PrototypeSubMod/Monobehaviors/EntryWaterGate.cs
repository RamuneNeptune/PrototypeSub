using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

[RequireComponent(typeof(Collider))]
internal class EntryWaterGate : MonoBehaviour
{
    [SerializeField] private SubRoot subRoot;

    private void OnTriggerEnter(Collider col)
    {
        var player = UWE.Utils.GetComponentInHierarchy<Player>(col.gameObject);

        if (!player || player.currChair != null) return;

        Vector3 dirToGate = (player.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToGate);

        if(dot < 0)
        {
            player.SetCurrentSub(subRoot, true);
        }
        else if(dot > 0)
        {
            player.SetCurrentSub(null, true);
        }
    }
}
