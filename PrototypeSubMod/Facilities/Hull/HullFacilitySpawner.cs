using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.Facilities.Hull;

public class HullFacilitySpawner : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}