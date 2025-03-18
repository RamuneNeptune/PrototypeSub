using PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class SetPrefabMaterialProperty : MonoBehaviour
{
    [SerializeField] private SpawnPrefabAtRuntime prefabSpawner;
    [SerializeField] private MaterialData[] materialDatas;

    [SerializeField, HideInInspector] private string[] propertyNames;
    [SerializeField, HideInInspector] private string[] childPaths;
    [SerializeField, HideInInspector] private int[] materialIndices;
    [SerializeField, HideInInspector] private MaterialData.PropertyType[] propertyTypes;
    [SerializeField, HideInInspector] private float[] floatValues;
    [SerializeField, HideInInspector] private Vector4[] vectorValues;
    [SerializeField, HideInInspector] private Texture[] textureValues;
    [SerializeField, HideInInspector] private Color[] colorValues;

    private MaterialData[] serializedMaterialDatas;

    private void OnValidate()
    {
        propertyNames = new string[materialDatas.Length];
        childPaths = new string[materialDatas.Length];
        materialIndices = new int[materialDatas.Length];
        propertyTypes = new MaterialData.PropertyType[materialDatas.Length];
        floatValues = new float[materialDatas.Length];
        vectorValues = new Vector4[materialDatas.Length];
        textureValues = new Texture[materialDatas.Length];
        colorValues = new Color[materialDatas.Length];

        for (int i = 0; i < materialDatas.Length; i++)
        {
            var data = materialDatas[i];
            propertyNames[i] = data.propertyName;
            childPaths[i] = data.childPath;
            materialIndices[i] = data.materialIndex;
            propertyTypes[i] = data.type;
            floatValues[i] = data.floatValue;
            vectorValues[i] = data.vectorValue;
            textureValues[i] = data.textureValue;
            colorValues[i] = data.colorValue;
        }
    }

    private void Start()
    {
        ReconstructMaterialDatas();
        prefabSpawner.OnSpawn += OnSpawn;
    }

    private void OnSpawn(GameObject obj)
    {
        foreach (var data in serializedMaterialDatas)
        {
            var renderer = obj.transform.Find(data.childPath)?.GetComponent<Renderer>();
            if (!renderer) throw new System.Exception($"No renderer found at child path {data.childPath} under {obj}");

            var materials = renderer.materials;
            switch (data.type)
            {
                case MaterialData.PropertyType.Float:
                    materials[data.materialIndex].SetFloat(data.propertyName, data.floatValue);
                    break;
                case MaterialData.PropertyType.Vector:
                    materials[data.materialIndex].SetVector(data.propertyName, data.vectorValue);
                    break;
                case MaterialData.PropertyType.Texture:
                    materials[data.materialIndex].SetTexture(data.propertyName, data.textureValue);
                    break;
                case MaterialData.PropertyType.Color:
                    materials[data.materialIndex].SetColor(data.propertyName, data.colorValue);
                    break;
            }
        }
    }

    private void ReconstructMaterialDatas()
    {
        for (int i = 0; i < propertyNames.Length; i++)
        {
            serializedMaterialDatas[i] = new MaterialData();
            var data = serializedMaterialDatas[i];
            data.propertyName = propertyNames[i];
            data.childPath = childPaths[i];
            data.materialIndex = materialIndices[i];
            data.type = propertyTypes[i];
            data.floatValue = floatValues[i];
            data.vectorValue = vectorValues[i];
            data.textureValue = textureValues[i];
            data.colorValue = colorValues[i];
        }
    }
}

public struct MaterialData
{
    public string name;
    public string propertyName;
    public string childPath;
    public int materialIndex;
    public PropertyType type;
    public float floatValue;
    public Vector4 vectorValue;
    public Texture textureValue;
    public Color colorValue;

    public enum PropertyType
    {
        Float,
        Vector,
        Texture,
        Color
    }
}