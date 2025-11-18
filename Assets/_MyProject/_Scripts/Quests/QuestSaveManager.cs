using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class QuestSaveManager : MonoBehaviour
{
    public static QuestSaveManager Instance;

    // A private key to save our data against in PlayerPrefs.
    private const string CompletedQuestsKey = "CompletedQuests";

    // A fast-lookup collection of all completed quest IDs loaded from PlayerPrefs.
    private HashSet<string> completedQuestIDs = new HashSet<string>();

    private void Awake()
    {
        // Standard singleton pattern.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCompletedQuests(); // Load quests as soon as the manager is ready.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads the list of completed quest IDs from PlayerPrefs.
    /// </summary>
    private void LoadCompletedQuests()
    {
        // Get the saved string from PlayerPrefs, which is a single line of IDs separated by commas.
        string savedData = PlayerPrefs.GetString(CompletedQuestsKey, "");

        if (!string.IsNullOrEmpty(savedData))
        {
            // Split the string back into individual IDs and add them to our HashSet.
            List<string> ids = savedData.Split(',').ToList();
            completedQuestIDs = new HashSet<string>(ids);
            Debug.Log($"[QuestSaveManager] Loaded {completedQuestIDs.Count} completed quests.");
        }
    }

    /// <summary>
    /// Saves the list of completed quest IDs to PlayerPrefs.
    /// </summary>
    /// <param name="completedQuests">The list of completed QuestData objects from the GameManager.</param>
    public void SaveCompletedQuests(List<QuestData> completedQuests)
    {
        // Convert the list of QuestData objects into a list of their unique IDs.
        List<string> idsToSave = completedQuests.Select(q => q.questID).ToList();

        // Join the list of IDs into a single string, separated by commas.
        string dataToSave = string.Join(",", idsToSave);

        // Save the string to PlayerPrefs.
        PlayerPrefs.SetString(CompletedQuestsKey, dataToSave);
        PlayerPrefs.Save(); // Force a save to disk.

        // Also update the local HashSet to match.
        completedQuestIDs = new HashSet<string>(idsToSave);

        Debug.Log($"[QuestSaveManager] Saved {idsToSave.Count} completed quests.");
    }

    /// <summary>
    /// Checks if a quest is marked as completed in our save data.
    /// </summary>
    /// <param name="questID">The unique ID of the quest to check.</param>
    /// <returns>True if the quest has been completed, false otherwise.</returns>
    public bool IsQuestCompleted(string questID)
    {
        if (string.IsNullOrEmpty(questID)) return false;
        return completedQuestIDs.Contains(questID);
    }
    public void ClearDataInMemory()
    {
        completedQuestIDs.Clear();
        Debug.Log("[QuestSaveManager] In-memory quest completion data has been cleared.");
    }
}