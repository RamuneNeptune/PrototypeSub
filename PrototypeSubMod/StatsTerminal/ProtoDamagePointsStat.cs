using SubLibrary.CyclopsReferencers;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoDamagePointsStat : MonoBehaviour, IStatistic, ICyclopsReferencer
{
    [SerializeField] private BehaviourLOD lod;
    [SerializeField] private Transform damagePointsParent;
    [SerializeField] private Transform displayPointsParent;

    [SerializeField, HideInInspector] public GameObject damagePointPrefab;
    private Dictionary<GameObject, GameObject> activeDamagePoints = new();

    private void Start()
    {
        foreach (Transform child in displayPointsParent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var iconPrefab = cyclops.transform.Find("HolographicDisplay").GetComponent<CyclopsHolographicHUD>().damageIconPrefab;
        damagePointPrefab = Instantiate(iconPrefab);
        damagePointPrefab.SetActive(false);

        var warningPing = damagePointPrefab.GetComponent<CyclopsHolographicHUD_WarningPings>();
        var protoPing = damagePointPrefab.AddComponent<ProtoWarningPing>();
        protoPing.warningType = warningPing.warningType;
        protoPing.damageText = warningPing.damageText;
        protoPing.labelDot = warningPing.labelDot;
        protoPing.warningPing = warningPing.warningPing;
        protoPing.lineRenderer = warningPing.lineRenderer;
        protoPing.animator = warningPing.animator;

        Destroy(warningPing);
    }

    public void UpdateStat() { }

    public void UpdateStatIntermittent()
    {
        for (int i = 0; i < damagePointsParent.childCount; i++)
        {
            var point = damagePointsParent.GetChild(i).gameObject;
            var displayChild = displayPointsParent.GetChild(i).gameObject;
            if (activeDamagePoints.TryGetValue(point, out GameObject controlledPing))
            {
                if (!point.activeSelf)
                {
                    activeDamagePoints.Remove(point);
                    controlledPing.GetComponent<ProtoWarningPing>().DespawnIcon();
                }
            }
            else if (point.activeSelf)
            {
                var ping = Instantiate(damagePointPrefab, displayChild.transform, false);
                activeDamagePoints.Add(point, ping);

                var protoPing = ping.GetComponent<ProtoWarningPing>();
                protoPing.SetLOD(lod);
                protoPing.SetParent(transform);
                ping.gameObject.SetActive(true);
            }
        }
    }
}
