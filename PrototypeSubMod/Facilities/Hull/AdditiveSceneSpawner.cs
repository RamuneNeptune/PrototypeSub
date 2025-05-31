using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.Facilities.Hull;

public class AdditiveSceneSpawner : MonoBehaviour
{
    public event Action onSceneLoaded;
    
    [SerializeField] private string sceneName;

    private bool cancelLoad;
    
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return op;
        onSceneLoaded?.Invoke();
    }

    /// <summary>
    /// This method will only work if called the first frame the script is awake, before the scene is started loading
    /// </summary>
    public void CancelLoad()
    {
        cancelLoad = true;
    }
}