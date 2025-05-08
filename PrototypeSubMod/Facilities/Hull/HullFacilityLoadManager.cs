using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Hull;

public class HullFacilityLoadManager : MonoBehaviour
{
    [SerializeField] private float loadDistance1;
    [SerializeField] private float loadDistance2;
    [SerializeField] private GameObject[] loadDistance1Objects;
    [SerializeField] private GameObject[] loadDistance2Objects;
    [SerializeField] private float checkInterval;

    private bool distance1Loaded;
    private bool distance2Loaded;
    
    private void Start()
    {
        SetObjectsActive(loadDistance1Objects, false);
        SetObjectsActive(loadDistance2Objects, false);
        StartCoroutine(UpdateStatus());
        UpdateObjectsActive();
    }

    private IEnumerator UpdateStatus()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(checkInterval);

            UpdateObjectsActive();
        }
    }

    private void UpdateObjectsActive()
    {
        float sqrDistance = (Camera.main.transform.position - gameObject.transform.position).sqrMagnitude;
        bool inRange1 = sqrDistance < loadDistance1 * loadDistance1;
        if (inRange1 != distance1Loaded)
        {
            SetObjectsActive(loadDistance1Objects, inRange1);
            distance1Loaded = inRange1;
        }
            
        bool inRange2 = sqrDistance < loadDistance2 * loadDistance2;
        if (inRange2 != distance2Loaded)
        {
            SetObjectsActive(loadDistance2Objects, inRange2);
            distance2Loaded = inRange2;
        }
    }

    private void SetObjectsActive(GameObject[] objects, bool active)
    {
        foreach (var obj in objects)
        {
            obj.SetActive(active);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loadDistance1);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loadDistance2);
    }
}