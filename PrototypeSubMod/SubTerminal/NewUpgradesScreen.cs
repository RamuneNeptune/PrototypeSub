using PrototypeSubMod.Utility;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using PrototypeSubMod.Prefabs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class NewUpgradesScreen : MonoBehaviour
{
    [SerializeField] private List<ProtoUpgradeCategory> upgradeCategories;
    [SerializeField] private BuildTerminalScreenManager screenManager;
    [SerializeField] private ProtoUpgradeCategory hullUpgradeCategory;
    [SerializeField] private VoiceNotificationManager manager;
    [SerializeField] private string storyEndPDAKey;
    [SerializeField] private string hullKeyPDAKey;
    [SerializeField] private VoiceNotification newDataNotification;
    [SerializeField] private string precursorCharacters;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private GameObject buttonObjects;
    [SerializeField] private GameObject downloadingObjects;
    [SerializeField] private Image progressBar;
    [SerializeField] private float downloadLength;

    private List<string> queuedPdaMessages = new();
    private List<ProtoUpgradeCategory> mostRecentCategories;
    private float currentDownloadProgress;
    private bool downloadActive;
    private bool pingSpawnAttempted;

    private Queue<VoiceNotification> queuedVoicelines = new();
    
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
                text += replacedString;
                if (index < mostRecentCategories.Count - 1)
                {
                    text += "\n";
                }

                index++;
            }

            upgradeText.text = text;
        }
        else if (!pingSpawnAttempted)
        {
            pingSpawnAttempted = true;
            downloadActive = false;
            screenManager.EnableRelevantScreensAtStart();
            queuedVoicelines.Clear();
            queuedVoicelines.Enqueue(newDataNotification);
            SpawnPingIfNeeded();
            UWE.CoroutineHost.StartCoroutine(PlayQueuedVoicelines());
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
            if (category.GetUnlockedUpgrades().Count > 0 && !Plugin.GlobalSaveData.unlockedCategoriesLastCheck.Contains(category.localizationKey))
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
        List<string> unlockedCategoryKeys = new();
        foreach (var category in GetUnlocksSinceLastCheck())
        {
            unlockedCategoryKeys.Add(category.localizationKey);
        }
        
        Plugin.GlobalSaveData.unlockedCategoriesLastCheck.AddRange(unlockedCategoryKeys);
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
        CheckForStoryPing();
        CheckForHullKey();
        
        screenManager.EndBuildStage();
    }

    private void CheckForHullKey()
    {
        if (KnownTech.Contains(HullFacilityKey.prefabInfo.TechType)) return;
        
        foreach (var item in upgradeCategories)
        {
            if (item == hullUpgradeCategory) continue;
            
            if (!Plugin.GlobalSaveData.unlockedCategoriesLastCheck.Contains(item.localizationKey)) return;
        }

        PDALog.Add(hullKeyPDAKey);
        KnownTech.Add(HullFacilityKey.prefabInfo.TechType);
        PDAEncyclopedia.Add("HullFacilityTabletEncy", true);
        queuedPdaMessages.Add(hullKeyPDAKey);
    }
    
    private void CheckForStoryPing()
    {
        if (Plugin.GlobalSaveData.storyEndPingSpawned)
        {
            return;
        }

        foreach (var item in upgradeCategories)
        {
            if (!Plugin.GlobalSaveData.unlockedCategoriesLastCheck.Contains(item.localizationKey)) return;
        }

        Plugin.GlobalSaveData.storyEndPingSpawned = true;
        UWE.CoroutineHost.StartCoroutine(SpawnStoryEndPing());
        PDALog.Add(storyEndPDAKey);
        queuedPdaMessages.Add(storyEndPDAKey);
    }
    
    private IEnumerator SpawnStoryEndPing()
    {
        var task = CraftData.GetPrefabForTechTypeAsync(Plugin.StoryEndPingTechType);
        yield return task;

        var prefab = task.GetResult();
        Instantiate(prefab, Plugin.STORY_END_POS, Quaternion.identity);
    }

    private IEnumerator PlayQueuedVoicelines()
    {
        float delay = 0;
        foreach (var message in queuedPdaMessages)
        {
            var data = Language.main.GetMetaData(message);
            for (int i = 0; i < data.lineCount; i++)
            {
                delay += data.GetLine(i).duration;
            }
        }

        yield return new WaitForSeconds(delay);
        
        while (queuedVoicelines.Count > 0)
        {
            var line = queuedVoicelines.Dequeue();
            manager.PlayVoiceNotification(line, false, true);

            yield return new WaitForSeconds(line.minInterval);
        }
    }

    private void OnEnable() => ResetDownload();
}
