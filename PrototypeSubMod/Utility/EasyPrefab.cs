using UnityEngine;

namespace PrototypeSubMod.Utility;

[CreateAssetMenu(fileName = "EasyPrefab", menuName = "Prototype Sub/Easy Prefab")]
internal class EasyPrefab : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprite;

    public DummyTechType techType;
    public bool applySNShaders;
    public bool unlockAtStart;

    [Tooltip("Tech type appended to the end of the path with extenson. Root folder is the Recipes folder")]
    public string jsonRecipePath;
}
