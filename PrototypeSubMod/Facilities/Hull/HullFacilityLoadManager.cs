using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.Facilities.Hull;

public class HullFacilityLoadManager : MonoBehaviour
{
    private const string HULL_FACILITY_SCENE_NAME = "protohullfacility";
    
    [SerializeField] private float loadDistance1;
    [SerializeField] private float loadDistance2;
    [SerializeField] private GameObject[] loadDistance1Objects;
    [SerializeField] private GameObject[] loadDistance2Objects;
    [SerializeField] private float checkInterval;
    [SerializeField] private bool manageScene;

    private string originalScene;
    private bool distance1Loaded;
    private bool distance2Loaded;
    
    private void Start()
    {
        originalScene = SceneManager.GetActiveScene().name;
        
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
        }
            
        bool inRange2 = sqrDistance < loadDistance2 * loadDistance2;
        if (inRange2 != distance2Loaded)
        {
            SetObjectsActive(loadDistance2Objects, inRange2);
        }

        if (manageScene)
        {
            bool rangeCheck = loadDistance1 > loadDistance2 ? inRange1 : inRange2;
            bool wasRangeCheck = loadDistance1 > loadDistance2 ? distance1Loaded : distance2Loaded;
            if (rangeCheck != wasRangeCheck)
            {
                UpdateActiveScene(rangeCheck);
            }
        }
        
        distance1Loaded = inRange1;
        distance2Loaded = inRange2;
    }

    private void UpdateActiveScene(bool inRange)
    {
        Plugin.Logger.LogInfo($"Updating active scene. In range = {inRange}");
        if (inRange && SceneManager.GetActiveScene().name != HULL_FACILITY_SCENE_NAME)
        {
            Plugin.Logger.LogInfo($"Setting active scene to hull facility");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(HULL_FACILITY_SCENE_NAME));
        }
        else if (!inRange && SceneManager.GetActiveScene().name == HULL_FACILITY_SCENE_NAME)
        {
            Plugin.Logger.LogInfo($"Setting active scene to main");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(originalScene));
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