using System;
using System.Collections;
using PrototypeSubMod.PathCreation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PrototypeSubMod.Facilities.Hull;

public class ProtoWormAnimator : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private ProtoWormSpineManager spineManager;
    [SerializeField] private Transform spineSegmentsParent;
    [SerializeField] private float speed;
    
    private bool moving;
    private float distanceMoved;
    private float offsetPerSpine;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
        pathCreator.path.UpdateTransform(pathCreator.transform);
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => spineManager.GetSpawned());
        offsetPerSpine = spineManager.GetIncrementPerSpine().z;
        
        moving = true;
    }

    private void Update()
    {
        if (!moving || !pathCreator) return;
        
        distanceMoved += speed * Time.deltaTime * Random.Range(0.5f, 1.5f);
        transform.position = GetPositionAtTime(distanceMoved, out var headRotation);
        transform.rotation = headRotation;
        
        int childCount = spineSegmentsParent.childCount;
        for (int i = 0; i < childCount; i += 3)
        {
            var segment1 = spineSegmentsParent.GetChild(i);
            var segment2 = spineSegmentsParent.GetChild(Mathf.Min(i + 1, childCount - 1));
            var segment3 = spineSegmentsParent.GetChild(Mathf.Min(i + 2, childCount - 1));
            
            float spineDistance1 = distanceMoved + spineManager.GetInitialLocalPos().z + offsetPerSpine * i;
            segment1.position = GetPositionAtTime(spineDistance1, out var rotation1);
            segment1.rotation = rotation1;
            
            float spineDistance3 = distanceMoved + spineManager.GetInitialLocalPos().z + offsetPerSpine * (i + 2);
            segment3.position = GetPositionAtTime(spineDistance3, out var rotation3);
            segment3.rotation = rotation3;
            
            segment2.position = (segment1.position + segment3.position) / 2;
            segment2.rotation = Quaternion.Slerp(segment1.rotation, segment3.rotation, 0.5f);
        }
    }

    private Vector3 GetPositionAtTime(float time, out Quaternion rotation)
    {
        var initialPoint = pathCreator.path.GetPointAtDistance(time);
        rotation = pathCreator.path.GetRotationAtDistance(time);
        return initialPoint;
    }

    public float GetNormalizedProgress()
    {
        return distanceMoved / pathCreator.path.length;
    }

    public float GetTimeForWormLength()
    {
        float spineLength = Mathf.Abs(spineManager.GetIncrementPerSpine().z) * spineManager.GetSpineSegmentCount() +
                            Mathf.Abs(spineManager.GetInitialLocalPos().z);
        return spineLength / speed;
    }
}