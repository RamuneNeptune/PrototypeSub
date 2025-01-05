using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class HeatAreaDistanceMultiplier : MonoBehaviour
{
    [SerializeField] private float multiplier;

    public float GetMultiplier()
    {
        return 1 / multiplier;
    }
}
