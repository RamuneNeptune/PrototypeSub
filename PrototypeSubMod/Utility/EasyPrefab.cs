using UnityEngine;

namespace PrototypeSubMod.Utility;

[CreateAssetMenu(fileName = "EasyPrefab", menuName = "Easy Prefab")]
internal class EasyPrefab : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprite;

    public string techTypeName;
    public bool applySNShaders;
    public bool unlockAtStart;
}
