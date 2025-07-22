using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class ProtoWormAnimator : MonoBehaviour
{
    [SerializeField] private ProtoWormSpineManager spineManager;
    [SerializeField] private GameObject headObject;
    [SerializeField] private Transform spineSegmentsParent;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [Range(0, 1)]
    [SerializeField] private float gizmoDrawLength;

    private List<FollowPoint> followPoints = new();
    private float distMoved;
    private float spineIncrement;
    private float travelledAngle;
    private int neededSegmentsLastFrame;
    private float endDistance;
    private float fullyDisabledDist;
    
    private void Start()
    {
        spineIncrement = spineManager.GetIncrementPerSpine().z;
        endDistance = speed * 36;
        fullyDisabledDist = endDistance + Mathf.Abs(spineIncrement) * spineManager.GetSpineSegmentCount();
    }

    private void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
        distMoved += speed * Time.deltaTime;
        transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0, 0), Space.Self);
        travelledAngle += rotationSpeed * Time.deltaTime;

        float absIncrement = Mathf.Abs(spineIncrement);
        float progress = (distMoved % absIncrement) / absIncrement;
        if (Mathf.FloorToInt(distMoved / spineIncrement) != neededSegmentsLastFrame)
        {
            Vector3 spawnPoint = transform.position - transform.forward * absIncrement;
            followPoints.Add(new FollowPoint(spawnPoint, transform.rotation));
            
            if (followPoints.Count > spineSegmentsParent.childCount + 1) followPoints.RemoveAt(0);
        }
        
        for (int i = 0; i < spineSegmentsParent.childCount; i++)
        {
            var child = spineSegmentsParent.GetChild(i);

            if (i >= followPoints.Count - 1)
            {
                child.gameObject.SetActive(false);
                child.position = followPoints[0].position;
                continue;
            }
            
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                child.GetComponentInChildren<Animator>(true).SetTrigger("StartMoving");
            }

            if (distMoved + spineIncrement * i >= endDistance)
            {
                child.gameObject.SetActive(false);
                continue;
            }

            UpdateSpineSegment(child, i, progress);
        }

        if (distMoved >= endDistance)
        {
            headObject.SetActive(false);
        }
        
        neededSegmentsLastFrame = Mathf.FloorToInt(distMoved / spineIncrement);
    }

    private void UpdateSpineSegment(Transform child, int index, float progress)
    {
        int invertedIndex = followPoints.Count - index - 1;
        var prevPoint = followPoints[invertedIndex - 1];
        var targetPoint = followPoints[invertedIndex];
        child.position = Vector3.Lerp(targetPoint.position, prevPoint.position, 1 - progress);
        
        child.rotation = Quaternion.Lerp(prevPoint.rotation, targetPoint.rotation, progress);
    }

    public float GetDistanceMoved() => distMoved;
    public float GetWormLength() => (spineManager.GetSpineSegmentCount() + 1) * -spineIncrement / speed;
    public float GetTravelledAngle() => travelledAngle;
    public float GetRotationSpeed() => rotationSpeed;
    public float RotationProgress() => distMoved / (fullyDisabledDist + speed);
    public void SetRotationSpeed(float speed) => rotationSpeed = speed;

    // Add a little extra distance to make sure it's fully done
    public bool DoneRotating() => distMoved >= fullyDisabledDist + speed;
    public bool HeadIsDisabled() => distMoved >= endDistance;
    
    private class FollowPoint
    {
        public Vector3 position;
        public Quaternion rotation;

        public FollowPoint(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 point = transform.position;
        Vector3 rotation = transform.forward;
        bool test = Vector3.Dot(Quaternion.AngleAxis(speed, transform.right) * transform.forward, Vector3.up) <
                    Vector3.Dot(Quaternion.AngleAxis(-speed, transform.right) * transform.forward, Vector3.up);
        Gizmos.color = test ? Color.green : Color.red;
        int increments = Mathf.FloorToInt(360 / rotationSpeed * gizmoDrawLength);
        for (int i = 0; i < increments; i++)
        {
            Gizmos.DrawRay(point, rotation * speed);
            point += rotation * speed;
            rotation = Quaternion.AngleAxis(rotationSpeed, transform.right) * rotation;
        }
    }
}