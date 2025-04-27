using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class ProtoWormSpineManager : MonoBehaviour
{
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
            StartCoroutine(StartAnimDelayed(spine, i * 0.5f));
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