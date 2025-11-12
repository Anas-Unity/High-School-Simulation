using UnityEngine;
using Invector.vCharacterController;

public class NPCDialogueTrigger : MonoBehaviour, Interactable
{
    [Header("Dialogue Logic")]
    [Tooltip("The dialogue to show BEFORE the associated quest is completed.")]
    public DialogueData dialogueData; // This is the default/quest-giving dialogue
    [Tooltip("The dialogue to show AFTER the associated quest is completed. (Optional)")]
    public DialogueData dialogueAfterCompletion; // This is the "thank you" or idle dialogue

    [Header("Associated Quest")]
    [Tooltip("The quest this NPC's dialogue is related to.")]
    public QuestData associatedQuest;

    [Header("UI References")]
    public GameObject promptUI;
    public GameObject dialoguePanel;

    [HideInInspector] public bool IsDialogueOpen { get; private set; }

    private InteractionTrigger interactionTrigger;
    private vThirdPersonController controller;
    private vThirdPersonInput controllerInput;
    private Rigidbody playerRB;

    [Header("Navigation Settings")]
    public bool enableNavigationAfterDialogue = false;
    [Tooltip("Index in NavigationManager's target list (e.g. 0 = School, 1 = Market, etc.)")]
    public int navigationTargetIndex = -1;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        interactionTrigger = GetComponent<InteractionTrigger>();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            controller = player.GetComponent<vThirdPersonController>();
            controllerInput = player.GetComponent<vThirdPersonInput>();
            playerRB = player.GetComponent<Rigidbody>();
        }
    }

    //public void Interact()
    //{
    //    if (IsDialogueOpen || dialogueData == null)
    //        return;

    //    // Start Dialogue
    //    DialogueManager.Instance.StartDialogue(dialogueData);

    //    if (dialoguePanel != null)
    //        dialoguePanel.SetActive(true);

    //    IsDialogueOpen = true;

    //    if (promptUI != null)
    //        promptUI.SetActive(false);

    //    LockPlayer(true);

    //    interactionTrigger?.DisablePrompt();

    //    DialogueManager.Instance.onDialogueEnd += CloseDialogue;
    //}

    public void Interact()
    {
        if (IsDialogueOpen) return;

        // 1. Decide which dialogue to show.
        DialogueData dialogueToShow = dialogueData; // Default to the main dialogue.

        // Check if there is an associated quest AND if it has been completed.
        if (associatedQuest != null && QuestSaveManager.Instance.IsQuestCompleted(associatedQuest.questID))
        {
            // If a special "after completion" dialogue exists, use it instead.
            if (dialogueAfterCompletion != null)
            {
                dialogueToShow = dialogueAfterCompletion;
            }
        }

        // 2. If there's a valid dialogue to show, start it.
        if (dialogueToShow != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToShow);
            // ... (The rest of the logic for locking player, etc.)
            LockPlayer(true);
            IsDialogueOpen = true;
            if (promptUI != null) promptUI.SetActive(false);
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            DialogueManager.Instance.onDialogueEnd += OnDialogueClosed;
        }
    }

    private void OnDialogueClosed()
    {
        IsDialogueOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        LockPlayer(false);
        DialogueManager.Instance.onDialogueEnd -= OnDialogueClosed;

        // --- QUEST LOGIC ---
        // Give or complete the quest ONLY if it's not already completed.
        if (associatedQuest != null && !QuestSaveManager.Instance.IsQuestCompleted(associatedQuest.questID))
        {
            // This logic assumes the NPC can either give the quest or complete it.
            // If the quest is already active, complete it. Otherwise, add it.
            if (GameManager.gameManager.activeQuests.Contains(associatedQuest))
            {
                GameManager.gameManager.CompleteQuest(associatedQuest);
            }
            else
            {
                GameManager.gameManager.AddQuest(associatedQuest);
            }
        }
    }

    //public void CloseDialogue()
    //{
    //    IsDialogueOpen = false;

    //    if (dialoguePanel != null)
    //        dialoguePanel.SetActive(false);

    //    LockPlayer(false);

    //    DialogueManager.Instance.onDialogueEnd -= CloseDialogue;
    //    interactionTrigger?.EnablePrompt();

    //    // --- QUEST LOGIC ---
    //    if (questToComplete != null)
    //        GameManager.gameManager.CompleteQuest(questToComplete);

    //    if (questToGive != null)
    //        GameManager.gameManager.AddQuest(questToGive);

    //    // --- NAVIGATION TRIGGER LOGIC ---
    //    if (enableNavigationAfterDialogue && navigationTargetIndex >= 0)
    //    {
    //        if (NavigationManager.nevigationManager != null)
    //        {
    //            NavigationManager.nevigationManager.SetDestination(navigationTargetIndex);

    //            // Automatically enable navigation after dialogue ends
    //            NavigationManager.nevigationManager.EnableNavigation(true);

    //            Debug.Log($"🧭 Navigation activated towards target index {navigationTargetIndex}");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("⚠️ NavigationManager not found in scene.");
    //        }
    //    }
    //}

    private void LockPlayer(bool state)
    {
        if (controller != null)
            controller.lockMovement = state;

        if (controllerInput != null)
            controllerInput.enabled = !state;

        if (playerRB != null)
        {
            if (state)
            {
                playerRB.velocity = Vector3.zero;
                playerRB.angularVelocity = Vector3.zero;
                playerRB.isKinematic = true;
            }
            else
            {
                playerRB.isKinematic = false;
            }
        }
    }
}
