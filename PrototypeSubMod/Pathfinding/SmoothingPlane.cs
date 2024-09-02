using UnityEngine;

namespace PrototypeSubMod.Pathfinding;

public struct SmoothingPlane
{
    private Vector3 pointOnPlane;
    private Vector3 originalNormal;
    private bool approachSide;

    private Mesh quadMesh;
    private Transform parent;
    private Vector3 originOffset;

    private Plane checkPlane
    {
        get
        {
            return _checkPlane;
        }
        set
        {
            Vector3 targetPoint = pointOnPlane;
            Vector3 newNormal = originalNormal;

            if (parent != null)
            {
                targetPoint = parent.TransformPoint(pointOnPlane - parent.position) + parent.position;
                newNormal = parent.TransformDirection(newNormal);
            }

            _checkPlane = new Plane(newNormal, targetPoint + originOffset);
        }
    }

    private Plane _checkPlane;

    public SmoothingPlane(Vector3 turningPoint, Vector3 pointOnPlane, Vector3 normal)
    {
        this.pointOnPlane = pointOnPlane;
        originalNormal = normal;

        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quadMesh = quad.GetComponent<MeshFilter>().mesh;
        GameObject.Destroy(quad);

        approachSide = false;
        parent = null;
        originOffset = Vector3.zero;
        _checkPlane = new Plane();
        checkPlane = default;

        approachSide = !checkPlane.GetSide(turningPoint);
    }

    public void UpdatePlane(Vector3 offsetFromGenPos, Transform relativeTo)
    {
        originOffset = offsetFromGenPos;
        parent = relativeTo;
        checkPlane = new Plane();
    }

    public bool GetSide(Vector3 point)
    {
        return checkPlane.GetSide(point);
    }

    public bool HasCrossedPlane(Vector3 point)
    {
        if (parent != null)
        {
            point = parent.TransformPoint(point);
        }

        return !checkPlane.GetSide(point);
    }

    public void DrawWithGizmos(Vector3 checkPoint, Vector3 gridOffset, Transform relativeTo, float scale)
    {
        parent = relativeTo;
        checkPlane = default;

        Vector3 targetPoint = pointOnPlane;
        Vector3 normal = checkPlane.normal;
        if (parent != null)
        {
            targetPoint = parent.TransformPoint(pointOnPlane - parent.position);
        }

        checkPoint = parent.TransformPoint(checkPoint);

        Gizmos.DrawSphere(parent.InverseTransformPoint(checkPoint), 0.2f);
        Gizmos.color = HasCrossedPlane(checkPoint) ? Color.green : Color.red;
        Gizmos.DrawWireMesh(quadMesh, targetPoint, Quaternion.LookRotation(normal), Vector3.one * scale);
    }
}
