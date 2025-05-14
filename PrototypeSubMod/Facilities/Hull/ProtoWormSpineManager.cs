using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class ProtoWormSpineManager : MonoBehaviour
{
    [SerializeField] private SkyApplier skyApplier;
    [SerializeField] private Transform segmentsParent;
    [SerializeField] private GameObject spineSegmentPrefab;
    [SerializeField] private Vector3 initialLocalPos;
    [SerializeField] private Vector3 incrementPerSpine;
    [SerializeField] private int spineSegmentCount;

    private bool spawned;
    
    private void Start()
    {
        for (int i = 0; i < spineSegmentCount; i++)
        {
            var spine = Instantiate(spineSegmentPrefab, segmentsParent);
            spine.transform.localPosition = initialLocalPos + incrementPerSpine * i;
            StartCoroutine(StartAnimDelayed(spine, i * 0.05f));

            if (skyApplier)
            {
                skyApplier.renderers.AddRangeToArray(spine.GetComponentsInChildren<Renderer>());
            }
        }

        if (skyApplier)
        {
            skyApplier.ApplySkybox();
        }

        spawned = true;
    }

    private IEnumerator StartAnimDelayed(GameObject spine, float delay)
    {
        yield return new WaitForSeconds(delay);

        spine.GetComponentInChildren<Animator>().SetTrigger("StartMoving");
    }

    public bool GetSpawned() => spawned;
    public Vector3 GetInitialLocalPos() => initialLocalPos;
    public Vector3 GetIncrementPerSpine() => incrementPerSpine;
}