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

    private void Start()
    {
        // Optional: Auto-load a starter quest if it exists
        QuestData rabbitQuest = Resources.Load<QuestData>("Q_Rabbit_TalkToRabbit");
        if (rabbitQuest != null && !activeQuests.Contains(rabbitQuest))
        {
            AddQuest(rabbitQuest);
        }
    }

    // ==============================
    // 🔸 QUEST MANAGEMENT
    // ==============================

    public void AddQuest(QuestData quest)
    {
        if (quest == null)
        {
            Debug.LogWarning("[GameManager] Tried to add a null quest.");
            return;
        }

        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            Debug.Log($"[GameManager] Added quest: {quest.questTitle}");
            OnQuestAdded?.Invoke(quest);
        }
        else
        {
            Debug.LogWarning($"[GameManager] Quest '{quest.questTitle}' already active.");
        }
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

            Debug.Log($"[GameManager] Completed quest: {quest.questTitle}");
            OnQuestCompleted?.Invoke(quest);
        }
        else
        {
            Debug.LogWarning($"[GameManager] Tried to complete a quest that isn’t active: {quest.questTitle}");
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
