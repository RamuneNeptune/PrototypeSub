using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PrototypePowerSystem : MonoBehaviour
{
    public static readonly string[] SLOT_NAMES = new string[]
    {
        "PrototypePowerSlot1",
        "PrototypePowerSlot2",
        "PrototypePowerSlot3",
        "PrototypePowerSlot4"
    };

    public static readonly TechType[] AllowedPowerSources = new[]
    {
        TechType.PowerCell,
        TechType.PrecursorIonCrystal,
        TechType.PrecursorIonPowerCell
    };

    public Equipment equipment { get; private set; }

    [SerializeField] private Transform storageRoot; 

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (equipment != null) return;

        equipment = new(gameObject, storageRoot);
        equipment.SetLabel("PrototypePowerLabel");
        equipment.onEquip += OnEquip;
        equipment.onUnequip += OnUnequip;

        equipment.AddSlots(SLOT_NAMES);

        equipment.isAllowedToAdd = IsAllowedToAdd;
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        Plugin.Logger.LogInfo($"Equipped {item} to slot {slot} on {gameObject}");
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        Plugin.Logger.LogInfo($"Unequipped {item} from slot {slot} on {gameObject}");
    }

    public void OnHover(HandTargetEventData eventData)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UseDecoyTube", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(equipment);
        pda.Open(PDATab.Inventory);
    }

    public TechType[] GetAllowedTechTypes()
    {
        return AllowedPowerSources;
    }

    private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
    {
        Plugin.Logger.LogInfo($"Trying to add {pickupable}");
        return AllowedPowerSources.Contains(pickupable.GetTechType());
    }
}
