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
    [SerializeField] private float offsetSpeed;
    [SerializeField] private float offsetAmplitude;
    
    private Transform[] spineSegments;
    private bool moving;
    private float distanceMoved;

    private void Start()
    {
        UWE.CoroutineHost.StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => spineManager.GetSpawned());

        spineSegments = new Transform[spineSegmentsParent.childCount];
        for (int i = 0; i < spineSegmentsParent.childCount; i++)
        {
            spineSegments[i] = spineSegmentsParent.GetChild(i);
        }
        moving = true;
    }

    private void Update()
    {
        if (!moving || !pathCreator) return;
        
        distanceMoved += speed * Time.deltaTime * Random.Range(0.5f, 1.5f);
        transform.position = GetPositionAtTime(distanceMoved, out var headRotation);
        transform.rotation = headRotation;

        int index = 0;
        foreach (var segment in spineSegments)
        {
            float spineDistance = distanceMoved + spineManager.GetInitialLocalPos().z + spineManager.GetIncrementPerSpine().z * index;
            segment.position = GetPositionAtTime(spineDistance, out var rotation);
            segment.rotation = rotation;
            
            index++;
        }
    }

    private Vector3 GetPositionAtTime(float time, out Quaternion rotation)
    {
        var initialPoint = pathCreator.path.GetPointAtDistance(time);
        var nextPoint = pathCreator.path.GetPointAtDistance(time + 0.01f);
        rotation = pathCreator.path.GetRotationAtDistance(time);
        var normal = pathCreator.path.GetNormalAtDistance(time);
        
        var side = Vector3.Cross((nextPoint - initialPoint).normalized, normal.normalized);
        return initialPoint + side * (Mathf.Sin(time * offsetSpeed) * offsetAmplitude);
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