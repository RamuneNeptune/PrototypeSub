using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
//
[CustomEditor(typeof(AddressablePreview))]
public class AddressablePreview_Editor : Editor
{
    private const int MAX_CHILD_ITERATION_DEPTH = 8;

    private AdvancedStringOptionsDropdown keysDropdown;
    private AdvancedStringOptionsDropdown valuesDropdown;
    private AdvancedStringOptionsDropdown childObjDropdown;

    private bool refreshValuesDropdown;
    private bool refreshChildren;

    private AddressablePreview preview;

    private static bool errorOnLoad;
    private static bool isSpawning;

    private IEnumerator spawnCoroutine;

    public override void OnInspectorGUI()
    {
        if (preview == null)
        {
            preview = target as AddressablePreview;
            preview.Initialize();
            preview.onUpdate = OnUpdate;
        }

        Color previousCol = preview.gizmoColor;
        bool previousDrawFrame = preview.drawWireframe;
        bool previousDisplayChild = preview.displayChild;

        preview.gizmoColor = EditorGUILayout.ColorField("Gizmo color", preview.gizmoColor);
        preview.drawWireframe = GUILayout.Toggle(preview.drawWireframe, "Draw wireframe");
        preview.displayChild = GUILayout.Toggle(preview.displayChild, "Display selected child");

        bool hasChanged = previousCol != preview.gizmoColor || previousDrawFrame != preview.drawWireframe;
        hasChanged |= previousDisplayChild != preview.displayChild;

        if (hasChanged)
        {
            SceneView.RepaintAll();
        }

        if(keysDropdown == null)
        {
            preview.keyIndex = -1;
        }

        if(GUILayout.Button("Clear references (Should be done before saving)"))
        {
            preview.parentObj = null;
            preview.childObj = null;
            preview.children.Clear();
            preview.keyIndex = -1;

            refreshChildren = true;
            refreshValuesDropdown = true;
        }

        if (GUILayout.Button("Select resource group"))
        {
            if(AddressablePreview.ResourceLocations == null)
            {
                preview.Initialize();
            }

            if (keysDropdown == null && AddressablePreview.ResourceLocations != null)
            {
                string[] keysAsStrings = AddressablePreview.ResourceLocations.Keys.ToArray();

                keysDropdown = new AdvancedStringOptionsDropdown(keysAsStrings);
                keysDropdown.OnOptionSelected += OnKeysDropdownSelected;
            }

            keysDropdown.Show(new Rect(200, 200, 500, 400));
        }

        if (preview.keyIndex != -1 && GUILayout.Button("Select resource preview"))
        {
            if (valuesDropdown == null || refreshValuesDropdown)
            {
                string[] valuesAsStrings = GetResourceLocationNames(AddressablePreview.ResourceLocations.ElementAt(preview.keyIndex).Value);

                valuesDropdown = new AdvancedStringOptionsDropdown(valuesAsStrings);
                valuesDropdown.OnOptionSelected += OnValueDropdownSelected;

                refreshValuesDropdown = false;
            }

            valuesDropdown.Show(new Rect(200, 600, 400, 400));
        }

        if (preview.resourceLocation != null)
        {
            HandleLoadAssetButton();
        }

        string text = preview.childObj ? $"Select child object ({preview.childObj.name})" : "Select child object";
        if (preview.parentObj != null && GUILayout.Button(text))
        {
            if(childObjDropdown == null || refreshChildren)
            {
                preview.children = new List<GameObject>();
                RecursivelyGetNames(preview.parentObj.transform, 0, new List<string>(), out var childNames);

                childObjDropdown = new AdvancedStringOptionsDropdown(childNames.ToArray());
                childObjDropdown.OnOptionSelected += ChildObjDropdown_OnOptionSelected;

                refreshChildren = false;
            }

            childObjDropdown.Show(new Rect(200, -200, 400, 400));
        }
    }

    private void RecursivelyGetNames(Transform parent, int previousIterationDepth, List<string> oldNames, out List<string> newNames)
    {
        newNames = oldNames;

        if(previousIterationDepth > MAX_CHILD_ITERATION_DEPTH)
        {
            Debug.LogWarning($"Max child iteration depth reached! Parent at depth {previousIterationDepth} = {parent}");
            return;
        }

        foreach (Transform child in parent)
        {
            string depthIdentifier = "";
            for (int i = 0; i < previousIterationDepth; i++)
            {
                depthIdentifier += "—";
            }

            string space = previousIterationDepth == 0 ? "" : " ";

            newNames.Add($"{depthIdentifier}{space}{child.name}");
            preview.children.Add(child.gameObject);

            if(child.childCount > 0)
            {
                RecursivelyGetNames(child, previousIterationDepth + 1, newNames, out List<string> childNames);
                newNames = childNames;
            }
        }

        previousIterationDepth++;
    }

    private void HandleLoadAssetButton()
    {
        if (preview.resourceLocation == null) return;

        string[] previewNameSegments = preview.resourceLocation.ToString().Split('/');
        string buttonText = isSpawning && !errorOnLoad ? "Loading asset..." : $"Load \"{previewNameSegments[previewNameSegments.Length - 1]}\"";

        if (GUILayout.Button(buttonText))
        {
            if (isSpawning && !errorOnLoad) return;

            spawnCoroutine = LoadPreviewAsync(preview.resourceLocation);
            EditorCoroutineUtility.StartCoroutine(spawnCoroutine, preview);

            EditorApplication.QueuePlayerLoopUpdate();

            isSpawning = true;
        }
    }

    private void OnUpdate()
    {
        if (spawnCoroutine == null) return;

        spawnCoroutine.MoveNext();

        EditorApplication.QueuePlayerLoopUpdate();
    }

    private IEnumerator LoadPreviewAsync(IResourceLocation location)
    {
        AsyncOperationHandle handle = Addressables.LoadAssetAsync<GameObject>(location);

        yield return handle.Task;

        switch (handle.Status)
        {
            case AsyncOperationStatus.Succeeded:
                GameObject previewGO = handle.Result as GameObject;

                preview.parentObj = previewGO;
                errorOnLoad = false;
                refreshChildren = true;

                SceneView.RepaintAll();
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError($"Error loading preview addressable! Exception = {handle.OperationException}");
                preview.parentObj = null;
                errorOnLoad = true;
                break;
        }

        isSpawning = false;
        spawnCoroutine = null;
    }

    private void OnKeysDropdownSelected(int index)
    {
        preview.keyIndex = index;
        refreshValuesDropdown = true;
        preview.resourceLocation = null;
    }

    private void OnValueDropdownSelected(int index)
    {
        preview.resourceLocation = AddressablePreview.ResourceLocations.ElementAt(preview.keyIndex).Value[index];
    }

    private void ChildObjDropdown_OnOptionSelected(int obj)
    {
        preview.childObj = preview.children[obj];
        preview.originalChildPos = preview.childObj.transform.position;
        preview.originalChildRot = preview.childObj.transform.rotation;
        preview.originalChildScale = preview.childObj.transform.localScale;

        SceneView.RepaintAll();
    }

    private string[] GetResourceLocationNames(List<IResourceLocation> locations)
    {
        string[] result = new string[locations.Count];
        for (int i = 0; i < locations.Count; i++)
        {
            result[i] = locations[i].ToString();
        }
        
        return result;
    }

    public class AdvancedStringOptionsDropdown : AdvancedDropdown
    {
        private string[] _enumNames;

        public event Action<int> OnOptionSelected;

        public AdvancedStringOptionsDropdown(string[] stringOptions) : base(new AdvancedDropdownState())
        {
            _enumNames = stringOptions;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            OnOptionSelected?.Invoke(item.id);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("");

            for (int i = 0; i < _enumNames.Length; i++)
            {
                var item = new AdvancedDropdownItem(_enumNames[i])
                {
                    id = i
                };

                root.AddChild(item);
            }

            return root;
        }
    }
}
