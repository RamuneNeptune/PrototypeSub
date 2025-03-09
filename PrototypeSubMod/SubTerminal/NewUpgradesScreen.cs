using PrototypeSubMod.Utility;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class NewUpgradesScreen : MonoBehaviour
{
    [SerializeField] private List<ProtoUpgradeCategory> upgradeCategories;
    [SerializeField] private string precursorCharacters;
    [SerializeField] private TextMeshProUGUI upgradeText;
    [SerializeField] private GameObject buttonObjects;
    [SerializeField] private GameObject downloadingObjects;
    [SerializeField] private Image progressBar;
    [SerializeField] private float downloadLength;

    private List<ProtoUpgradeCategory> mostRecentCategories;
    private float currentDownloadProgress;
    private bool downloadActive;

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
    }
    
    private void StartDownload()
    {
        currentDownloadProgress = 0;
        downloadActive = true;
        buttonObjects.SetActive(false);
        downloadingObjects.SetActive(true);
        mostRecentCategories = GetUnlocksSinceLastCheck();
    }

    public List<ProtoUpgradeCategory> GetUnlocksSinceLastCheck()
    {
        var currentlyUnlocked = new List<TechType>();
        var newUnlocks = new List<ProtoUpgradeCategory>();

        foreach (var category in upgradeCategories)
        {
            var unlockedTechs = category.GetUnlockedUpgrades();
            foreach (var tech in unlockedTechs)
            {
                if (!Plugin.GlobalSaveData.unlockedUpgradesLastCheck.Contains(tech))
                {
                    newUnlocks.Add(category);
                    break;
                }
            }

            currentlyUnlocked.AddRange(unlockedTechs);
        }

        Plugin.GlobalSaveData.unlockedUpgradesLastCheck = currentlyUnlocked;
        return newUnlocks;
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
}
