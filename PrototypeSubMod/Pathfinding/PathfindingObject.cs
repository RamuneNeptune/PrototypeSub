using System;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Pathfinding;

public class PathfindingObject : MonoBehaviour
{
    public event Action OnPathFinished;

    [SerializeField] private PathRequestManager pathManager;
    [SerializeField] private Transform targetPoint;
    [SerializeField] protected Transform visual;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float turnDistance;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool moveEvenIfPathNotComplete;
    [SerializeField] private bool runPathfindingOnStart;
    [SerializeField] private bool goToEndIfFail;

    protected bool useLocalPos;
    protected PathfindingGrid grid;

    protected Path path;
    protected Vector3 directionToNextPoint;
    protected Vector3 lastNormal;
    protected bool movingAlongPath;
    
    private Vector3 originalTargetPos;

    private void Start()
    {
        grid = GetComponentInParent<PathfindingGrid>();
        useLocalPos = grid != null;

        if (runPathfindingOnStart)
        {
            PathRequest request = new PathRequest(transform.position, targetPoint.position, OnPathFound);
            pathManager.RequestPath(request);
        }
    }

    private void OnEnable()
    {
        grid = GetComponentInParent<PathfindingGrid>();
        useLocalPos = grid != null;
    }

    private void OnPathFound(PathData[] pathData, bool success)
    {
        if (goToEndIfFail && !success && pathData.Length == 0)
        {
            transform.position = originalTargetPos;
            OnPathFinished?.Invoke();
            return;
        }

        if (!moveEvenIfPathNotComplete && !success) return;

        path = new Path(pathData, transform.position, turnDistance);
        StopCoroutine(FollowPath());
        StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        if (path == null || path.pathData.Length <= 0) yield break;

        movingAlongPath = true;
        
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
            if (posToCheck == targetPointPosition || (posToCheck - targetPointPosition).sqrMagnitude < 2f)
            {
                targetIndex++;

                if (targetIndex >= path.pathData.Length)
                {
                    path = null;
                    visual.rotation = Quaternion.LookRotation(directionToNextPoint, lastNormal);
                    OnPathFinished?.Invoke();
                    movingAlongPath = false;
                    yield break;
                }

                currentWaypoint = path.pathData[targetIndex];

                targetPointPosition = currentWaypoint.position + offsetFromGenPos;
                if (useLocalPos)
                {
                    targetPointPosition = grid.root.TransformPoint(currentWaypoint.position);
                }
            }

            Vector3 currentPos = transform.position;
            if (useLocalPos) currentPos = grid.transform.TransformPoint(transform.localPosition);

            directionToNextPoint = targetPointPosition - currentPos;

            var newPos = Vector3.MoveTowards(currentPos, targetPointPosition, moveSpeed * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation(directionToNextPoint, lastNormal);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, targetRot, rotationSpeed * Time.deltaTime);

            transform.position = newPos;

            yield return null;
        }
    }

    public void UpdatePath()
    {
        originalTargetPos = targetPoint.position;
        PathRequest request = new PathRequest(transform.position, targetPoint.position, OnPathFound);
        pathManager.RequestPath(request);
    }

    public void UpdatePath(Vector3 position)
    {
        originalTargetPos = position;
        PathRequest request = new PathRequest(transform.position, position, OnPathFound);
        pathManager.RequestPath(request);
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

    public void SetPathfindingManager(PathRequestManager pathManager)
    {
        this.pathManager = pathManager;
    }

    public bool GetMovingAlongPath() => movingAlongPath;
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