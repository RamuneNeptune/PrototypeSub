using UnityEngine;

namespace PrototypeSubMod.Pathfinding;

public class Path
{
    public readonly PathData[] pathData;
    public readonly SmoothingPlane[] turnBoundaries;
    public readonly int finishLineIndex;

    public Path(PathData[] pathData, Vector3 startPos, float turnDist)
    {
        this.pathData = pathData;
        turnBoundaries = new SmoothingPlane[pathData.Length];
        finishLineIndex = pathData.Length - 1;

        Vector3 previousPoint = startPos;
        for (int i = 0; i < this.pathData.Length; i++)
        {
            Vector3 currentPoint = pathData[i].position;
            Vector3 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector3 turnBoundaryPoint = i == finishLineIndex ? currentPoint : currentPoint - (dirToCurrentPoint * turnDist);
            turnBoundaries[i] = new SmoothingPlane(currentPoint, turnBoundaryPoint, -dirToCurrentPoint);

            previousPoint = turnBoundaryPoint;
        }
    }

    public void DrawWithGizmos(Vector3 checkPoint, Vector3 gridOffset, Transform parent)
    {
        Gizmos.color = Color.black;
        foreach (var point in pathData)
        {
            Vector3 position = point.position;
            if (parent != null) position = parent.TransformPoint(position - parent.transform.position);

            Gizmos.DrawCube(position + gridOffset, Vector3.one * 0.5f);
        }

        Gizmos.color = Color.white;
        foreach (var plane in turnBoundaries)
        {
            //plane.DrawWithGizmos(checkPoint, gridOffset, parent, 5f);
        }
    }

    public void UpdatePlanes(Vector3 offset, Transform parent)
    {
        foreach (var plane in turnBoundaries)
        {
            plane.UpdatePlane(offset, parent);
        }
    }

    public Vector3 GetLocalPathPoint(int index, Transform relativeTo, Vector3 offset)
    {
        Vector3 position = pathData[index].position;
        if (relativeTo != null)
        {
            position = relativeTo.TransformPoint(position - relativeTo.transform.position);
        }

        return position + offset;
    }

    public Vector3 GetLocalPathNormal(int index, Transform relativeTo)
    {
        Vector3 normal = pathData[index].normal;
        if (relativeTo != null)
        {
            normal = relativeTo.TransformDirection(normal);
        }

        return normal;
    }
}
