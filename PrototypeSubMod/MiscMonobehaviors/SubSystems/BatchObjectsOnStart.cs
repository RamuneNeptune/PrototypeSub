using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class BatchObjectsOnStart : MonoBehaviour
{
    [SerializeField] private GameObject staticBatchRoot;

    private void Start()
    {
        StaticBatchingUtility.Combine(staticBatchRoot);
    }
}