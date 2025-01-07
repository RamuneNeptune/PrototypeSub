using SubLibrary.CyclopsReferencers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoDamagePointsStat : MonoBehaviour, IStatistic, ICyclopsReferencer
{
    [SerializeField] private BehaviourLOD lod;
    [SerializeField] private Transform damagePointsParent;
    [SerializeField] private Transform displayPointsParent;

    [SerializeField, HideInInspector] public GameObject damagePointPrefab;
    private List<ManagedDamagePoint> activeDamagePoints = new();

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
        foreach (var point in damagePointsParent.GetComponentsInChildren<CyclopsDamagePoint>(true))
        {
            var displayChild = displayPointsParent.GetChild(point.transform.GetSiblingIndex()).gameObject;
            ManagedDamagePoint damagePoint = activeDamagePoints.FirstOrDefault(i => i.damagePoint == point || (i.damagePoint == null && i.managedPoint != null));

            if (damagePoint != null && damagePoint.damagePoint == null && damagePoint.managedPoint != null)
            {
                RemovePing(damagePoint);
                continue;
            }

            if (point.gameObject.activeSelf && damagePoint == null)
            {
                SpawnDamagePing(displayChild.transform, point);
            }
            else if (!point.gameObject.activeSelf && damagePoint != null)
            {
                RemovePing(damagePoint);
            }
        }
    }

    private void SpawnDamagePing(Transform parent, CyclopsDamagePoint ownerPoint)
    {
        var ping = Instantiate(damagePointPrefab, parent, false);
        activeDamagePoints.Add(new ManagedDamagePoint(ownerPoint, ping.gameObject));

        Vector3 prevScale = transform.localScale;
        transform.localScale = Vector3.one;

        var protoPing = ping.GetComponent<ProtoWarningPing>();
        protoPing.SetLOD(lod);
        protoPing.SetParent(transform);
        ping.gameObject.SetActive(true);

        transform.localScale = prevScale;
    }

    private void RemovePing(ManagedDamagePoint damagePoint)
    {
        damagePoint.managedPoint.GetComponent<ProtoWarningPing>().DespawnIcon();
        activeDamagePoints.Remove(damagePoint);
    }

    private class ManagedDamagePoint
    {
        public CyclopsDamagePoint damagePoint;
        public GameObject managedPoint;

        public ManagedDamagePoint(CyclopsDamagePoint damagePoint, GameObject managedPoint)
        {
            this.damagePoint = damagePoint;
            this.managedPoint = managedPoint;
        }
    }
}
