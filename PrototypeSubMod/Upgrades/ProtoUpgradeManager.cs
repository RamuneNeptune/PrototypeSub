using UnityEngine;

namespace PrototypeSubMod.Upgrades;

internal class ProtoUpgradeManager : MonoBehaviour
{
    public static ProtoUpgradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void SetUpgradeActive()
    {

    }
}
