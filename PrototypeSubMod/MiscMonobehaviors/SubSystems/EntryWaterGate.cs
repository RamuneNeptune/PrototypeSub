using PrototypeSubMod.Patches;
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

        /*
        int positionsBehindGate = 0;
        int positionsInFrontOfGate = 0;

        for (int i = 0; i < Player_Patches.lastPlayerPositions.Length; i++)
        {
            Vector3 pos = Player_Patches.lastPlayerPositions[i];

            Vector3 dirToGate = (pos - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, dirToGate);

            if (dot > 0)
            {
                positionsInFrontOfGate++;
            }
            else if (dot < 0)
            {
                positionsBehindGate++;
            }
        }

        Vector3 currentPlayerPos = player.transform.position;
        Vector3 directionToGate = (currentPlayerPos - transform.position).normalized;
        float currentDot = Vector3.Dot(transform.forward, directionToGate);

        bool exitCheck1 = currentDot > 0 && positionsBehindGate > positionsInFrontOfGate;
        bool exitCheck2 = positionsBehindGate > positionsInFrontOfGate + Player_Patches.lastPlayerPositions.Length / 2;

        bool entryCheck1 = currentDot < 0 && positionsInFrontOfGate > positionsBehindGate;
        bool entryCheck2 = positionsInFrontOfGate > positionsBehindGate + Player_Patches.lastPlayerPositions.Length / 2;

        if (exitCheck1 || exitCheck2)
        {
            player.SetCurrentSub(null, true);
        }
        else if (entryCheck1 || entryCheck2)
        {
            player.SetCurrentSub(subRoot, true);
        }
        */
    }
}
