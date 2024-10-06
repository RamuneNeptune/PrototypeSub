using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Pathfinding;

public class PathfindingObject : MonoBehaviour
{
    public event Action OnPathFinished;

    [SerializeField] private Transform targetPoint;
    [SerializeField] protected Transform visual;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnDistance;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool moveEvenIfPathNotComplete;
    [SerializeField] private bool runPathfindingOnStart;

    protected bool useLocalPos;
    private PathfindingGrid grid;

    protected Path path;
    protected Vector3 directionToNextPoint;
    protected Vector3 lastNormal;
    protected Vector3 lastPointOnBounds;

    private Vector3 targetPointPos;

    private void Start()
    {
        grid = GetComponentInParent<PathfindingGrid>();
        useLocalPos = grid != null;

        if (runPathfindingOnStart)
        {
            PathRequest request = new PathRequest(transform.position, targetPoint.position, OnPathFound);
            PathRequestManager.RequestPath(request);
        }
    }

    private void OnEnable()
    {
        grid = GetComponentInParent<PathfindingGrid>();
        useLocalPos = grid != null;
    }

    private void OnPathFound(PathData[] pathData, bool success)
    {
        if (!moveEvenIfPathNotComplete && !success) return;

        path = new Path(pathData, transform.position, turnDistance);
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        if (path == null || path.pathData.Length <= 0) yield break;

        PathData currentWaypoint = path.pathData[0];
        directionToNextPoint = currentWaypoint.position - visual.position;
        int targetIndex = 0;

        while (true)
        {
            Vector3 offsetFromGenPos = Vector3.zero;
            if (useLocalPos)
            {
                offsetFromGenPos = grid.transform.position - grid.GetPositionAtGridGen();
                offsetFromGenPos = grid.root.TransformVector(offsetFromGenPos);
            }

            Vector3 targetPointPosition = currentWaypoint.position + offsetFromGenPos;
            if (useLocalPos)
            {
                targetPointPosition = grid.root.TransformPoint(currentWaypoint.position);
            }

            Vector3 posToCheck = transform.position;

            lastNormal = currentWaypoint.normal;
            if (useLocalPos) lastNormal = grid.root.TransformDirection(currentWaypoint.normal);
            if (posToCheck == targetPointPosition)
            {
                targetIndex++;

                if (targetIndex >= path.pathData.Length)
                {
                    path = null;
                    visual.rotation = Quaternion.LookRotation(directionToNextPoint, lastNormal);
                    OnPathFinished?.Invoke();
                    yield break;
                }

                currentWaypoint = path.pathData[targetIndex];

                targetPointPosition = currentWaypoint.position + offsetFromGenPos;
                if (useLocalPos)
                {
                    targetPointPosition = grid.root.TransformPoint(currentWaypoint.position);
                }
            }

            lastPointOnBounds = currentWaypoint.position;

            Vector3 currentPos = transform.position;
            if (useLocalPos) currentPos = grid.transform.TransformPoint(transform.localPosition);

            directionToNextPoint = targetPointPosition - currentPos;
            targetPointPos = targetPointPosition;

            var newPos = Vector3.MoveTowards(currentPos, targetPointPosition, moveSpeed * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation(directionToNextPoint, lastNormal);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, targetRot, rotationSpeed * Time.deltaTime);

            transform.position = newPos;

            yield return null;
        }
    }

    public void UpdatePath()
    {
        Plugin.Logger.LogInfo($"Requesting path");
        PathRequest request = new PathRequest(transform.position, targetPoint.position, OnPathFound);
        PathRequestManager.RequestPath(request);
    }

    public void UpdatePath(Vector3 position)
    {
        Plugin.Logger.LogInfo($"Requesting path");
        PathRequest request = new PathRequest(transform.position, position, OnPathFound);
        PathRequestManager.RequestPath(request);
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Vector3 offsetFromGenPos = Vector3.zero;
        if (useLocalPos)
        {
            offsetFromGenPos = grid.transform.position - grid.GetPositionAtGridGen();
            offsetFromGenPos = grid.transform.TransformVector(offsetFromGenPos);
        }

        Quaternion rotation = Quaternion.identity;
        if (useLocalPos)
        {
            rotation = grid.transform.rotation * Quaternion.Inverse(grid.GetRotationAtGridGen());
        }

        path.DrawWithGizmos(transform.position, offsetFromGenPos, grid?.transform);
    }

    /*
    for (int i = targetIndex; i < path.Length; i++)
        {
            Vector3 offsetFromGenPos = Vector3.zero;
            if (useLocalPos) offsetFromGenPos = grid.transform.position - grid.GetPositionAtGridGen();

            Gizmos.color = Color.black;
            Gizmos.DrawCube(path[i].position + offsetFromGenPos, Vector3.one * 0.2f);

            Vector3 pointA = path[i].position + offsetFromGenPos;
            Vector3 pointB = i != path.Length - 1 ? path[i + 1].position + offsetFromGenPos : path[i].position + offsetFromGenPos;

            Gizmos.DrawLine(pointA, pointB);
        } 
    */

    /*
    if (path == null || path.Length <= 0) yield break;

        PathData currentWaypoint = path[0];
        Vector3 dirToNext = currentWaypoint.position - visual.position;
        targetIndex = 0;

        while (true)
        {
            Vector3 offsetFromGenPos = Vector3.zero;
            if (useLocalPos)
            {
                offsetFromGenPos = grid.transform.position - grid.GetPositionAtGridGen();
            }

            Vector3 targetPointPosition = useLocalPos ? grid.GetWorldToLocalPoint(currentWaypoint.position + offsetFromGenPos) : currentWaypoint.position;

            Vector3 posToCheck = useLocalPos ? transform.localPosition : transform.position;
            if (posToCheck == targetPointPosition)
            {
                targetIndex++;

                if (targetIndex >= path.Length)
                {
                    path = null;
                    visual.rotation = Quaternion.LookRotation(dirToNext, currentWaypoint.normal);
                    yield break;
                }

                dirToNext = path[targetIndex].position - transform.position;
                currentWaypoint = path[targetIndex];
            }

            posToCheck = Vector3.MoveTowards(posToCheck, targetPointPosition, moveSpeed * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation(dirToNext, currentWaypoint.normal);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, targetRot, rotationSpeed * Time.deltaTime);

            if (useLocalPos)
            {
                transform.localPosition = posToCheck;
            }
            else
            {
                transform.position = posToCheck;
            }

            yield return null;
        } 
    */

    //Smooth path following. Can't get it to work with rotations
    /*
     bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.pathData[0].position);

        while(followingPath)
        {
            Vector3 offsetFromGenPos = Vector3.zero;
            if (useLocalPos) offsetFromGenPos = grid.transform.position - grid.GetPositionAtGridGen();

            path.UpdatePlanes(offsetFromGenPos, grid?.transform);

            Vector3 checkPoint = transform.position + offsetFromGenPos;
            if(useLocalPos)
            {
                checkPoint = grid.transform.TransformPoint(checkPoint);
            }

            while (path.turnBoundaries[pathIndex].HasCrossedPlane(checkPoint))
            {
                path.UpdatePlanes(offsetFromGenPos, grid?.transform);

                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if(followingPath)
            {
                Vector3 relativeLocalPos = grid.transform.TransformPoint(transform.position - grid.transform.position);

                Vector3 targetDir = path.GetLocalPathPoint(pathIndex, grid?.transform, offsetFromGenPos) - relativeLocalPos;
                Vector3 normal = path.GetLocalPathNormal(pathIndex, grid?.transform);

                //if (useLocalPos) targetDir += grid.transform.position;
                Debug.DrawRay(transform.position, targetDir, Color.green);
                Debug.DrawRay(transform.position, normal, Color.cyan);

                Quaternion targetRot = Quaternion.LookRotation(targetDir.normalized, normal);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * rotationSpeed);
                transform.localPosition += targetDir.normalized * Time.deltaTime * moveSpeed;
            }

            yield return null;
        }
    */
}

public struct PathData
{
    public Vector3 position;
    public Vector3 normal;

    public PathData(Vector3 point, Vector3 normal)
    {
        position = point;
        this.normal = normal;
    }
}