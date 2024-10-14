using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PowerSourceFunctionality : MonoBehaviour
{
    protected int powerSourceCount;

    public virtual void OnCountChanged(bool added)
    {
        powerSourceCount += added ? 1 : -1;

        if (powerSourceCount <= 0)
        {
            Destroy(this);
        }
    }
}
