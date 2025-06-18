using PrototypeSubMod.DeployablesTerminal;
using UnityEngine;

namespace PrototypeSubMod.UI.DeployablesDisplay;

public class ProtoDeployablesDisplay : MonoBehaviour
{
    [SerializeField] private DeployablesStorageTerminal deployablesStorage;
    [SerializeField] private Transform[] lightIcons;

    private void Start()
    {
        deployablesStorage.equipment.onAddItem += _ => UpdateIcons();
        deployablesStorage.equipment.onRemoveItem += _ => UpdateIcons();
        UpdateIcons();
    }

    public void UpdateIcons()
    {
        int lightCount = 0;
        for (int i = 0; i < DeployablesStorageTerminal.LightBeaconSlots.Length; i++)
        {
            bool hasLight = deployablesStorage.equipment.GetItemInSlot(DeployablesStorageTerminal.LightBeaconSlots[i]) != null;
            lightCount += hasLight ? 1 : 0;
        }
        
        for (int i = 0; i < lightIcons.Length; i++)
        {
            lightIcons[i].gameObject.SetActive(i <= lightCount - 1);
        }
    }
}