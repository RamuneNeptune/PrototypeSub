using PrototypeSubMod.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class NewUpgradesScreen : MonoBehaviour
{
    [SerializeField] private List<ProtoUpgradeCategory> upgradeCategories;
    [SerializeField] private ProtoUpgradeCategory defenseCategory;
    [SerializeField] private BuildTerminalScreenManager screenManager;
    [SerializeField] private VoiceNotificationManager manager;
    [SerializeField] private VoiceNotification defensePingNotification;
    [SerializeField] private VoiceNotification storyEndNotification;
    [SerializeField] private string precursorCharacters;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private GameObject buttonObjects;
    [SerializeField] private GameObject downloadingObjects;
    [SerializeField] private Image progressBar;
    [SerializeField] private float downloadLength;

    private List<ProtoUpgradeCategory> mostRecentCategories;
    private float currentDownloadProgress;
    private bool downloadActive;
    private bool pingSpawnAttempted;

    private void Update()
    {
        if (!downloadActive || mostRecentCategories == null || mostRecentCategories.Count == 0) return;

        if (Time.timeScale == 0) return;

        if (currentDownloadProgress < downloadLength)
        {
            currentDownloadProgress += Time.deltaTime;
            float normalizedProgress = currentDownloadProgress / downloadLength;
            progressBar.fillAmount = normalizedProgress;

            var text = "";
            int index = 0;
            foreach (var category in mostRecentCategories)
            {
                var replacedString = ReplaceWithPrecursorChars(category.GetName(), normalizedProgress);
                text += replacedString + "\n";
                if (mostRecentCategories.Count > 1 && index < mostRecentCategories.Count - 1)
                {
                    text += "———————\n";
                }

                index++;
            }

            upgradeText.text = text;
        }
        else if (!pingSpawnAttempted)
        {
            pingSpawnAttempted = true;
            SpawnPingIfNeeded();
        }
    }

    public void StartDownload()
    {
        upgradeText.text = string.Empty;
        progressBar.fillAmount = 0;
        currentDownloadProgress = 0;
        downloadActive = true;
        pingSpawnAttempted = false;
        buttonObjects.SetActive(false);
        downloadingObjects.SetActive(true);
        mostRecentCategories = GetUnlocksSinceLastCheck();
        UpdateStoredUnlocks();
    }

    public List<ProtoUpgradeCategory> GetUnlocksSinceLastCheck()
    {
        var newUnlocks = new List<ProtoUpgradeCategory>();

        foreach (var category in upgradeCategories)
        {
            if (category.GetUnlockedUpgrades().Count > 0 && !Plugin.GlobalSaveData.unlockedCategoriesLastCheck.Contains(category))
            {
                newUnlocks.Add(category);
            }
        }

        return newUnlocks;
    }

    public bool HasQueuedUnlocks()
    {
        return GetUnlocksSinceLastCheck().Count > 0;
    }

    private void UpdateStoredUnlocks()
    {
        Plugin.GlobalSaveData.unlockedCategoriesLastCheck.AddRange(GetUnlocksSinceLastCheck());
    }

    private string ReplaceWithPrecursorChars(string original, float amount)
    {
        int length = original.Length;
        int newLength = (int)Mathf.Lerp(0, length, amount);

        string replacementString = string.Empty;
        for (int i = 0; i < length - newLength; i++)
        {
            replacementString += precursorCharacters[Random.Range(0, precursorCharacters.Length - 1)];
        }

        return original[0..newLength] + replacementString;
    }

    public void ResetDownload()
    {
        downloadActive = false;
        buttonObjects.SetActive(true);
        downloadingObjects.SetActive(false);
    }

    private void SpawnPingIfNeeded()
    {
        CheckForDefensePing();
        CheckForStoryPing();
    }

    private void CheckForDefensePing()
    {
        if (Plugin.GlobalSaveData.defensePingSpawned)
        {
            screenManager.EndBuildStage();
            return;
        }

        foreach (var item in upgradeCategories)
        {
            if (item == defenseCategory) continue;

            if (!Plugin.GlobalSaveData.unlockedCategoriesLastCheck.Contains(item)) return;
        }

        Plugin.GlobalSaveData.defensePingSpawned = true;
        UWE.CoroutineHost.StartCoroutine(SpawnDefensePing());
        manager.PlayVoiceNotification(defensePingNotification, false, true);
        screenManager.EndBuildStage();
    }

    private void CheckForStoryPing()
    {
        if (Plugin.GlobalSaveData.storyEndPingSpawned)
        {
            screenManager.EndBuildStage();
            return;
        }

        foreach (var item in upgradeCategories)
        {
            if (!Plugin.GlobalSaveData.unlockedCategoriesLastCheck.Contains(item)) return;
        }

        Plugin.GlobalSaveData.storyEndPingSpawned = true;
        UWE.CoroutineHost.StartCoroutine(SpawnStoryEndPing());
        manager.PlayVoiceNotification(storyEndNotification, false, true);
        screenManager.EndBuildStage();
    }

    private IEnumerator SpawnDefensePing()
    {
        var task = CraftData.GetPrefabForTechTypeAsync(Plugin.DefenseFacilityPingTechType);
        yield return task;

        var prefab = task.GetResult();
        Instantiate(prefab, Plugin.DEFENSE_PING_POS, Quaternion.identity);
    }
    
    private IEnumerator SpawnStoryEndPing()
    {
        var task = CraftData.GetPrefabForTechTypeAsync(Plugin.StoryEndPingTechType);
        yield return task;

        var prefab = task.GetResult();
        Instantiate(prefab, Plugin.STORY_END_POS, Quaternion.identity);
    }

    private void OnEnable() => ResetDownload();
}
