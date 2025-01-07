using SubLibrary.CyclopsReferencers;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.StatsTerminal;

internal class ProtoLavaLarvaStat : MonoBehaviour, IStatistic, ICyclopsReferencer
{
    [SerializeField] private SubRoot root;
    [SerializeField] private Transform lavaLarvaParent;
    [SerializeField] private Transform larvaIconsParent;

    [SerializeField, HideInInspector] public GameObject larvaPointPrefab;
    private Dictionary<GameObject, ProtoWarningPing> larvaPings = new();

    public void OnCyclopsReferenceFinished(GameObject cyclops)
    {
        var iconPrefab = cyclops.transform.Find("HolographicDisplay").GetComponent<CyclopsHolographicHUD>().lavaLarvaIconPrefab;
        larvaPointPrefab = Instantiate(iconPrefab);
        larvaPointPrefab.SetActive(false);

        var warningPing = larvaPointPrefab.GetComponent<CyclopsHolographicHUD_WarningPings>();
        var protoPing = larvaPointPrefab.AddComponent<ProtoWarningPing>();
        protoPing.CopyFromWarningPing(warningPing);

        Destroy(warningPing);
    }

    public void UpdateStat() { }

    public void UpdateStatIntermittent() { }

    public void AttachedLavaLarva(GameObject go)
    {
        Vector3 position = larvaIconsParent.InverseTransformPoint(go.GetComponent<AttachToVehicle>().currentAttachPoint.transform.position) * 0.04f;
        var icon = Instantiate(larvaPointPrefab);

        icon.transform.SetParent(larvaIconsParent);
        icon.transform.localPosition = position;
        icon.transform.localScale = Vector3.one * 0.004f;

        var ping = icon.GetComponent<ProtoWarningPing>();
        ping.SetLOD(root.LOD);
        ping.SetParent(larvaIconsParent);

        icon.gameObject.SetActive(true);

        larvaPings.Add(go, ping);
    }

    public void DetachedLavaLarva(GameObject go)
    {
        if (larvaPings.TryGetValue(go, out ProtoWarningPing ping))
        {
            Destroy(ping.gameObject);
            larvaPings.Remove(go);
        }
    }
}
