using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

[RequireComponent(typeof(Collider))]
internal class EntryWaterGate : MonoBehaviour
{
    private const float MIN_TIME_BETWEEN_ENTRIES = 0.15f;
    private static EntryInfo LastEntryInfo;

    [SerializeField] private bool setWalk;
    [SerializeField] private SubRoot subRoot;

    private void OnTriggerEnter(Collider col)
    {
        bool firstEntry = LastEntryInfo.lastEntryTime == -1;
        bool cooldownMet = LastEntryInfo.lastEntryTime + MIN_TIME_BETWEEN_ENTRIES <= Time.time;
        if (!firstEntry && !cooldownMet && LastEntryInfo.entering == setWalk)
        {
            return;
        }

        var player = UWE.Utils.GetComponentInHierarchy<Player>(col.gameObject);

        if (!player || player.currChair != null) return;

        if (!subRoot)
        {
            player.SetPrecursorOutOfWater(setWalk);
            return;
        }

        player.SetCurrentSub(setWalk ? subRoot : null, true);
    }

    private struct EntryInfo
    {
        public float lastEntryTime = -1;
        public bool entering;

        public EntryInfo(float lastEntryTime, bool entering)
        {
            this.lastEntryTime = lastEntryTime;
            this.entering = entering;
        }
    }
}
