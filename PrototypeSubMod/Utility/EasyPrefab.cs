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
}
