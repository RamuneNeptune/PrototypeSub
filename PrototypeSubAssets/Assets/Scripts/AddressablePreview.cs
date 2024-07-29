using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
//
[ExecuteAlways]
public class AddressablePreview : MonoBehaviour
{
    public static Dictionary<string, List<IResourceLocation>> ResourceLocations;
    private static bool Initialized;

    public Action onUpdate;
    public GameObject parentObj;
    public GameObject childObj;
    public List<GameObject> children = new List<GameObject>();

    public bool drawWireframe = true;
    public bool displayChild = true;
    public int keyIndex = -1;

    public Vector3 originalChildPos;
    public Quaternion originalChildRot;
    public Vector3 originalChildScale;

    public Color gizmoColor = Color.white;
    public IResourceLocation resourceLocation;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        onUpdate?.Invoke();
    }

    public void Initialize()
    {
        if(Initialized && ResourceLocations != null) return;

        List<IResourceLocation> resourceLocations = LoadResourceLocations();

        ResourceLocations = resourceLocations.GroupBy(GroupLocation).ToDictionary(g => g.Key, g => g.Distinct().ToList());
        Initialized = true;
    }

    private List<IResourceLocation> LoadResourceLocations()
    {
        var set = new HashSet<IResourceLocation>();
        if(!Addressables.ResourceLocators.Any()) return set.ToList();

        foreach (var locator in Addressables.ResourceLocators)
        {
            if (!(locator is ResourceLocationMap rlm)) continue;

            foreach (var location in rlm.Locations.SelectMany(loc => loc.Value).Where(val => val.ResourceType != typeof(IAssetBundleResource)))
            {
                set.Add(location);
            }
        }

        return set.ToList();
    }

    private string GroupLocation(IResourceLocation g)
    {
        if (g.ResourceType == typeof(SceneInstance))
        {
            return "Scenes";
        }
        else
        {
            var lastIndexForwardslash = g.PrimaryKey.LastIndexOf("/");
            var lastIndexBackslash = g.PrimaryKey.LastIndexOf("\\");
            if (lastIndexForwardslash > -1 || lastIndexBackslash > -1)
            {
                if (lastIndexBackslash > lastIndexForwardslash)
                    return g.PrimaryKey.Substring(0, lastIndexBackslash);
                else
                    return g.PrimaryKey.Substring(0, lastIndexForwardslash);
            }
            else
                return "Assorted";
        }
    }

    private void OnDrawGizmos()
    {
        if (!parentObj && !childObj) return;

        GameObject currentObj = childObj != null && displayChild ? childObj : parentObj;

        currentObj.transform.position = transform.position;
        currentObj.transform.rotation = transform.rotation;
        currentObj.transform.localScale = transform.localScale;

        if(childObj)
        {
            childObj.SetActive(displayChild);
        }

        MeshFilter[] meshFilters = currentObj.GetComponentsInChildren<MeshFilter>(true);
        Gizmos.color = gizmoColor;

        foreach (var filter in meshFilters)
        {
            if(drawWireframe)
            {
                Gizmos.DrawWireMesh(filter.sharedMesh, filter.transform.position, filter.transform.rotation, filter.transform.localScale);
            }
            else
            {
                Gizmos.DrawMesh(filter.sharedMesh, filter.transform.position, filter.transform.rotation, filter.transform.localScale);
            }
        }
    }
}
