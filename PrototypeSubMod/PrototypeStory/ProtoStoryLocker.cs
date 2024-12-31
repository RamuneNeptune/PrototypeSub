using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class ProtoStoryLocker : MonoBehaviour
{
    public static bool StoryEndingActive;

    [SerializeField] private LiveMixin mixin;
    [SerializeField] private ProtoUpgradeManager upgradeManager;
    [SerializeField] private float activationDistance;
    [SerializeField] private float checkInterval;

    private void Start()
    {
        InvokeRepeating(nameof(CheckForDistance), 0, checkInterval);
    }

    private void CheckForDistance()
    {
        float dist = (Plugin.STORY_END_POS - transform.position).sqrMagnitude;
        if (dist < (activationDistance * activationDistance))
        {
            OnEnterStoryEnding();
            CancelInvoke(nameof(CheckForDistance));
        }
    }

    private void OnEnterStoryEnding()
    {
        StoryEndingActive = true;
        mixin.invincible = true;

        ErrorMessage.AddError("Entered story ending; locking upgrades");
        foreach (var upgrade in upgradeManager.GetInstalledUpgrades())
        {
            upgrade.SetUpgradeEnabled(false);
            upgrade.SetUpgradeLocked(true);
        }
    }

    private void OnDestroy()
    {
        StoryEndingActive = false;
    }
}
