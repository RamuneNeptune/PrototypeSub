using PrototypeSubMod.Utility;
using UnityEngine;

namespace PrototypeSubMod.Facilities;

internal class UnlockProtoUpgrade : MonoBehaviour
{
    [SerializeField] private MultipurposeAlienTerminal terminal;
    [SerializeField] private ProtoUpgradeCategory upgradeCategory;
    [SerializeField] private DummyTechType techType;
    [SerializeField] private string encyclopediaKey;

    private bool unlocked;

    private void Start()
    {
        unlocked = KnownTech.Contains(techType.TechType);
        terminal.onTerminalInteracted += UnlockTechType;

        if (unlocked)
        {
            terminal.ForceInteracted();
        }
    }

    public void UnlockTechType()
    {
        if (unlocked) return;

        KnownTech.Add(techType.TechType);

        int lockedCount = upgradeCategory.GetLockedUpgrades().Count;
        string message = Language.main.GetFormat("ProtoUpgradeUnlocked", Language.main.Get(techType.TechType), lockedCount);

        if (lockedCount == 0)
        {
            message = Language.main.GetFormat("ProtoUpgradeSetComplete", Language.main.Get(techType.TechType));
        }
  
        ErrorMessage.AddError(message);

        if (!string.IsNullOrEmpty(encyclopediaKey))
        {
            PDAEncyclopedia.Add(encyclopediaKey, true);
        }

        unlocked = true;
    }
}
