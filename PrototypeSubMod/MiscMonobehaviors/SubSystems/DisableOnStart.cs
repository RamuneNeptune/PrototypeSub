using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.SubSystems;

public class DisableOnStart : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
}