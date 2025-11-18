using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    [Header("Currency")]
    public int coins = 500;

    [Header("Active Quests")]
    public List<QuestData> activeQuests = new List<QuestData>();

    [Header("Completed Quests")]
    public List<QuestData> completedQuests = new List<QuestData>();

    [Header("Quest Settings")]
    [Tooltip("The very first quest to be automatically assigned when the game starts.")]
    public QuestData startingQuest;

    // 🔹 EVENTS
    public event Action<QuestData> OnQuestAdded;
    public event Action<QuestData> OnQuestRemoved;
    public event Action<QuestData> OnQuestCompleted;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = this;
            DontDestroyOnLoad(gameManager);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Save quests when the player quits the game.
    private void OnApplicationQuit()
    {
        if (QuestSaveManager.Instance != null)
        {
            QuestSaveManager.Instance.SaveCompletedQuests(completedQuests);
        }
    }   

    private void Start()
    {
        // Auto-assign the starter quest if it's set and not already completed.
        if (startingQuest != null)
        {
            // We use the AddQuest logic which already checks if it's completed or active.
            AddQuest(startingQuest);
        }
    }

    // ==============================
    // 🔸 QUEST MANAGEMENT
    // ==============================

    public void AddQuest(QuestData quest)
    {
        if (quest == null) return;

        // --- ADD THIS CHECK ---
        // Do not add the quest if it's already completed or already active.
        if (QuestSaveManager.Instance.IsQuestCompleted(quest.questID) || activeQuests.Contains(quest))
        {
            Debug.LogWarning($"[GameManager] Quest '{quest.questTitle}' is already completed or active. Cannot add.");
            return;
        }
        // --- END NEW CHECK ---

        activeQuests.Add(quest);
        Debug.Log($"[GameManager] Added quest: {quest.questTitle}");
        OnQuestAdded?.Invoke(quest);
    }


    public void RemoveQuest(QuestData quest)
    {
        if (quest == null) return;

        if (activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            Debug.Log($"[GameManager] Removed quest: {quest.questTitle}");
            OnQuestRemoved?.Invoke(quest);
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        if (quest == null)
        {
            Debug.LogWarning("[GameManager] Tried to complete a null quest.");
            return;
        }

        if (activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            if (!completedQuests.Contains(quest))
            {
                completedQuests.Add(quest);
            }
            // Tell the Save Manager to save the new completed quest list.
            if (QuestSaveManager.Instance != null)
            {
                QuestSaveManager.Instance.SaveCompletedQuests(completedQuests);
            }

            Debug.Log($"[GameManager] Completed quest: {quest.questTitle}");
            OnQuestCompleted?.Invoke(quest);

            // Check if there is a follow-up quest and activate it.
            if (quest.nextQuest != null)
            {
                AddQuest(quest.nextQuest);
                Debug.Log($"[GameManager] Automatically activated next quest: {quest.nextQuest.questTitle}");
            }
        }
        else
        {
            Debug.LogWarning($"[GameManager] Tried to complete a quest that isn’t active: {quest.questTitle}");
        }
    }

    /// Resets the live quest state by clearing all active and completed quest lists.
    /// This is essential for a full game reset.
    public void ResetQuestState()
    {
        activeQuests.Clear();
        completedQuests.Clear();
        Debug.Log("[GameManager] Live quest state (active and completed lists) has been reset.");

        // Optional but recommended: Re-add the starting quest after a reset.
        if (startingQuest != null)
        {
            AddQuest(startingQuest);
        }
    }

    // ==============================
    // 🔸 CURRENCY MANAGEMENT
    // ==============================

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        coins += amount;
        Debug.Log($"[GameManager] Added {amount} coins. Total: {coins}");
    }

    public void SpendCoins(int amount)
    {
        if (amount <= 0) return;
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log($"[GameManager] Spent {amount} coins. Remaining: {coins}");
        }
        else
        {
            Debug.LogWarning($"[GameManager] Not enough coins. Current balance: {coins}");
        }
    }
}
