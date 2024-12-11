using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

internal class Proximity3dViewer : MonoBehaviour
{
    [SerializeField] private Transform targetPointsParent;
    [SerializeField] private Transform objToRotate;
    [SerializeField] private Transform rotationTarget;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool disableWhenNoPointsActive;

    private List<Transform> targets = new List<Transform>();

    private void Start()
    {
        foreach (Transform child in targetPointsParent)
        {
            targets.Add(child);
        }
    }

    private void Update()
    {
        if (!targets.Any(t => t.gameObject.activeSelf) && disableWhenNoPointsActive)
        {
            objToRotate.gameObject.SetActive(false);
            return;
        }
        else if (!objToRotate.gameObject.activeSelf)
        {
            objToRotate.gameObject.SetActive(true);
        }

        Vector3 center = GetAveragePositions(targets);
        Vector3 dirToTarget = (rotationTarget.position - objToRotate.position).normalized;

        Quaternion targetRotation = Quaternion.FromToRotation((center - objToRotate.position).normalized, dirToTarget) * objToRotate.rotation;

        Debug.DrawLine(objToRotate.position, center, Color.red);
        Debug.DrawLine(center, rotationTarget.position, Color.magenta);

        objToRotate.rotation = Quaternion.Slerp(objToRotate.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private Vector3 GetAveragePositions(List<Transform> list)
    {
        Vector3 sum = Vector3.zero;
        int activeItems = 0;

        foreach (Transform tr in list)
        {
            if (!tr.gameObject.activeSelf) continue;

            sum += tr.position;
            activeItems++;
        }

        if (activeItems == 0)
        {
            return Vector3.zero;
        }

        sum /= activeItems;

        return sum;
    }
}
