using System;
using System.Collections;
using Nautilus.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.Facilities.Hull;

public class ProtoFacilityLoadManager : MonoBehaviour
{
    [SerializeField] private float loadDistance1;
    [SerializeField] private float loadDistance2;
    [SerializeField] private GameObject[] loadDistance1Objects;
    [SerializeField] private GameObject[] loadDistance2Objects;
    [SerializeField] private float checkInterval;

    [SerializeField] private string sceneName;
    [SerializeField] private bool manageScene;

    private Material skybox;
    private string originalScene;
    private bool distance1Loaded;
    private bool distance2Loaded;
    
    private void Start()
    {
        originalScene = SceneManager.GetActiveScene().name;
        skybox = RenderSettings.skybox;

        Plugin.GlobalSaveData.OnStartedSaving += OnStartedSaving;
        Plugin.GlobalSaveData.OnFinishedSaving += OnFinishedSaving;
        
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
        if (!Camera.main) return;
        
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

        if (WaitScreen.IsWaiting) return;
        
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
        if (inRange && SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            RenderSettings.skybox = skybox;
        }
        else if (!inRange && SceneManager.GetActiveScene().name == sceneName)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(originalScene));
            RenderSettings.skybox = skybox;
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

    private void OnDestroy()
    {
        Plugin.GlobalSaveData.OnStartedSaving -= OnStartedSaving;
        Plugin.GlobalSaveData.OnFinishedSaving -= OnFinishedSaving;
    }

    private Scene previousScene;

    private void OnStartedSaving(object sender, JsonFileEventArgs args)
    {
        previousScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(originalScene));
    }
    
    private void OnFinishedSaving(object sender, JsonFileEventArgs args)
    {
        SceneManager.SetActiveScene(previousScene);
    }
}