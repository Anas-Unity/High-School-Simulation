using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestBookBehaviour : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject questPage;
    [SerializeField] private Transform questListContainer;
    [SerializeField] private GameObject questButtonPrefab;
    [SerializeField] private Text questDescriptionBox;       // Or TMP_Text if using TextMeshPro
    [SerializeField] private GameObject bookNotification;

    //[Header("Quest Display Settings")]
    //[Tooltip("The color used to highlight the Quest Giver's name.")]
    //public Color giverHighlightColor = Color.yellow; // Default color is set here

    [Header("Display Text")]
    [SerializeField] private string noQuestsText = "You have no active quests.";
    [SerializeField] private string[] defaultQuestHints;

    private bool isBookOpen = false;
    private QuestData selectedQuest;
    private readonly List<GameObject> questButtons = new();

    private void Awake()
    {
        if (questPage != null) questPage.SetActive(false);
        if (bookNotification != null) bookNotification.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.OnQuestAdded += OnQuestChanged;
            GameManager.gameManager.OnQuestRemoved += OnQuestChanged;
            GameManager.gameManager.OnQuestCompleted += OnQuestChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.OnQuestAdded -= OnQuestChanged;
            GameManager.gameManager.OnQuestRemoved -= OnQuestChanged;
            GameManager.gameManager.OnQuestCompleted -= OnQuestChanged;
        }
    }

    private void OnQuestChanged(QuestData quest)
    {
        RefreshBook();
        UpdateNotification();
    }

    // 🔹 Toggle book visibility
    public void ToggleQuestBook()
    {
        if (questPage == null) return;

        isBookOpen = !isBookOpen;
        questPage.SetActive(isBookOpen);

        if (isBookOpen)
        {
            if (bookNotification != null) bookNotification.SetActive(false);
            RefreshBook();
        }
    }

    public void ShowBook()
    {
        if (questPage == null) return;
        isBookOpen = true;
        questPage.SetActive(true);
        if (bookNotification != null) bookNotification.SetActive(false);
        RefreshBook();
    }

    public void HideBook()
    {
        if (questPage == null) return;
        isBookOpen = false;
        questPage.SetActive(false);
    }

    // 🔹 Refresh UI when quests change
    public void RefreshBook()
    {
        var gm = GameManager.gameManager;
        if (gm == null || gm.activeQuests == null)
        {
            questDescriptionBox.text = noQuestsText;
            ClearQuestList();
            return;
        }

        var quests = gm.activeQuests;
        ClearQuestList();

        if (quests.Count == 0)
        {
            questDescriptionBox.text = defaultQuestHints != null && defaultQuestHints.Length > 0
                ? defaultQuestHints[Random.Range(0, defaultQuestHints.Length)]
                : noQuestsText;
            return;
        }

        foreach (var quest in quests)
        {
            if (quest == null) continue;

            var buttonObj = Instantiate(questButtonPrefab, questListContainer);
            var questButtonUI = buttonObj.GetComponent<QuestButtonUI>();

            if (questButtonUI != null)
            {
                questButtonUI.SetTitle(quest.questTitle);
                questButtonUI.button.onClick.AddListener(() => ShowQuestDetails(quest));
            }
            else
            {
                Debug.LogWarning("[QuestBook] QuestButtonUI missing on prefab!");
            }

            questButtons.Add(buttonObj);
        }

        // Auto-select first quest if only one
        if (quests.Count == 1)
            ShowQuestDetails(quests[0]);
        else
            questDescriptionBox.text = "Select a quest to view its details.";
    }

    // 🔹 Show selected quest details
    public void ShowQuestDetails(QuestData quest)
    {
        if (quest == null)
        {
            questDescriptionBox.text = noQuestsText;
            return;
        }

        selectedQuest = quest;

        string giver = string.IsNullOrEmpty(quest.questGiverName) ? "Unknown" : quest.questGiverName;
        string description = string.IsNullOrEmpty(quest.questDescription) ? "No description available." : quest.questDescription;

        string formattedGiver = $"<color=#FFA500><b>From: {giver}</b></color>";

        // Combine the formatted giver line with the default description line.
        questDescriptionBox.text = $"{formattedGiver}\n\n{description}";

        HighlightSelectedQuestButton(quest);
    }

    /*public void ShowQuestDetails(QuestData quest)
    {
        if (quest == null)
        {
            questDescriptionBox.text = noQuestsText;
            return;
        }

        selectedQuest = quest;

        string giver = string.IsNullOrEmpty(quest.questGiverName) ? "Unknown" : quest.questGiverName;
        string description = string.IsNullOrEmpty(quest.questDescription) ? "No description available." : quest.questDescription;

        // CONVERSION STEP: Convert the public Color object to a hex string for the rich text tag.
        string hexColor = ColorUtility.ToHtmlStringRGB(giverHighlightColor);

        // Apply rich text tags using the dynamic hex color string
        string formattedGiver = $"<color=#{hexColor}><b>From: {giver}</b></color>";

        questDescriptionBox.text = $"{formattedGiver}\n\n{description}";

        // Assuming this method exists and needs the quest object
        HighlightSelectedQuestButton(quest);
    }*/

    private void HighlightSelectedQuestButton(QuestData quest)
    {
        foreach (var obj in questButtons)
        {
            var questButtonUI = obj.GetComponent<QuestButtonUI>();
            if (questButtonUI == null) continue;

            string title = questButtonUI.titleText != null ? questButtonUI.titleText.text :
                           questButtonUI.tmpTitleText != null ? questButtonUI.tmpTitleText.text : "";

            bool isSelected = title == quest.questTitle;

            if (questButtonUI.titleText != null)
                questButtonUI.titleText.color = isSelected ? Color.red : Color.black;
            if (questButtonUI.tmpTitleText != null)
                questButtonUI.tmpTitleText.color = isSelected ? Color.red : Color.black;
        }
    }

    private void ClearQuestList()
    {
        foreach (var obj in questButtons)
            if (obj != null) Destroy(obj);

        questButtons.Clear();
    }

    private void UpdateNotification()
    {
        if (bookNotification == null) return;

        var gm = GameManager.gameManager;
        bookNotification.SetActive(gm != null && gm.activeQuests != null && gm.activeQuests.Count > 0);
    }
}
