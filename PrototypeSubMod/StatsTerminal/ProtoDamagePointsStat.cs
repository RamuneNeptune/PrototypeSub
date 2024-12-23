using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoDamagePointsStat : MonoBehaviour, IStatistic
{
    [SerializeField] private Transform damagePointsParent;
    [SerializeField] private Transform displayPointsParent;

    private void Start()
    {
        foreach (Transform child in displayPointsParent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void UpdateStat() { }

    public void UpdateStatIntermittent()
    {
        for (int i = 0; i < damagePointsParent.childCount; i++)
        {
            var childA = damagePointsParent.GetChild(i);
            var childB = displayPointsParent.GetChild(i);

            childB.gameObject.SetActive(childA.gameObject.activeSelf);
        }
    }
}
