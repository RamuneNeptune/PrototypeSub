using System;
using System.Collections;
using PrototypeSubMod.PathCreation;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class ProtoWormAnimator : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private ProtoWormSpineManager spineManager;
    [SerializeField] private Transform spineSegmentsParent;
    [SerializeField] private float speed;
    
    private Transform[] spineSegments;
    private bool moving;
    private float distanceMoved;

    private IEnumerator Start()
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
        
        distanceMoved += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceMoved);
        transform.rotation =  pathCreator.path.GetRotationAtDistance(distanceMoved);

        int index = 0;
        foreach (var segment in spineSegments)
        {
            float spineDistance = distanceMoved + spineManager.GetInitialLocalPos().z + spineManager.GetIncrementPerSpine().z * index;
            segment.position = pathCreator.path.GetPointAtDistance(spineDistance);
            segment.rotation =  pathCreator.path.GetRotationAtDistance(spineDistance);
            index++;
        }
    }
}