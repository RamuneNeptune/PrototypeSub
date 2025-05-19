using PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

public class SetPrefabMaterialProperty : MonoBehaviour
{
    [SerializeField] private Component prefabSpawner;
    [SerializeField] private bool editSharedMaterials;
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
    private IMaterialModifier materialModifier;

    private void OnValidate()
    {
        if (prefabSpawner is not IMaterialModifier)
        {
            prefabSpawner = null;
        }
        
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

    private void Awake()
    {
        materialModifier = prefabSpawner as IMaterialModifier;
        ReconstructMaterialDatas();
        materialModifier.onEditMaterial += EditMaterials;
    }

    private void EditMaterials(GameObject obj)
    {
        foreach (var data in serializedMaterialDatas)
        {
            Renderer renderer = null;
            if (string.IsNullOrEmpty(data.childPath))
            {
                renderer = obj.GetComponent<Renderer>();
            }
            else
            {
                renderer = obj.transform.Find(data.childPath)?.GetComponent<Renderer>();
                if (!renderer) throw new System.Exception($"No renderer found at child path {data.childPath} under {obj}");
            }

            var materials = editSharedMaterials ? renderer.sharedMaterials : renderer.materials;
            if (data.materialIndex == -1)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    SetPropertyValue(ref materials, data, i);
                }
            }
            else
            {
                SetPropertyValue(ref materials, data, data.materialIndex);
            }

            if (editSharedMaterials)
            {
                renderer.sharedMaterials = materials;
            }
            else
            {
                renderer.materials = materials;
            }
        }
    }

    private void SetPropertyValue(ref Material[] materials, MaterialData data, int index)
    {
        switch (data.type)
        {
            case MaterialData.PropertyType.Float:
                materials[index].SetFloat(data.propertyName, data.floatValue);
                break;
            case MaterialData.PropertyType.Vector:
                materials[index].SetVector(data.propertyName, data.vectorValue);
                break;
            case MaterialData.PropertyType.Texture:
                materials[index].SetTexture(data.propertyName, data.textureValue);
                break;
            case MaterialData.PropertyType.Color:
                materials[index].SetColor(data.propertyName, data.colorValue);
                break;
        }
    }

    private void ReconstructMaterialDatas()
    {
        serializedMaterialDatas = new MaterialData[propertyNames.Length];

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

[System.Serializable]
public class MaterialData
{
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