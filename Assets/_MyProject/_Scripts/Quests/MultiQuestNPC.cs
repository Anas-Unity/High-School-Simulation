using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for advanced list operations like .LastOrDefault()

/// A helper class to organize quest data in the Inspector for the MultiQuestNPC.
/// This does not need to be in its own file.
[System.Serializable]
public class QuestAssignment
{
    [Tooltip("The QuestData asset for this stage of the NPC's story.")]
    public QuestData quest;

    [Tooltip("Dialogue to show when this quest is available to be given or is currently in progress.")]
    public DialogueData dialogueBeforeQuest;

    [Tooltip("Dialogue to show after this quest has been completed (can be used for idle chatter).")]
    public DialogueData dialogueAfterQuest;

    [Header("Post-Dialogue Actions")]
    [Tooltip("Check this to enable a navigation path after this quest stage is handled.")]
    public bool enableNavigation = false;
    [Tooltip("The index of the target in the NavigationManager's list.")]
    public int navigationTargetIndex = -1;
}

/// A stateful NPC component that can manage a sequence of quests and dialogues over time.
/// Replaces the simpler NPCDialogueTrigger for characters with evolving stories.
public class MultiQuestNPC : MonoBehaviour, Interactable
{
    [Header("Quest Progression")]
    [Tooltip("The list of quests this NPC will manage, in the order they should be offered.")]
    [SerializeField] private List<QuestAssignment> questProgression;

    [Header("UI References")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject dialoguePanel;

    // --- Private State Variables ---
    private bool isDialogueOpen = false;
    private QuestAssignment currentAssignment;

    // --- Player Controller References (for locking movement) ---
    // Make sure that player has the "Player" tag.
    private Invector.vCharacterController.vThirdPersonController playerController;
    private Invector.vCharacterController.vThirdPersonInput playerInput;

    public bool IsDialogueOpen => isDialogueOpen;

    private void Start()
    {
        // Find and store references to the player's components for locking/unlocking.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<Invector.vCharacterController.vThirdPersonController>();
            playerInput = player.GetComponent<Invector.vCharacterController.vThirdPersonInput>();
        }
        else
        {
            Debug.LogWarning($"[MultiQuestNPC] Player object not found! Tag a player with 'Player' for movement lock to work.", this);
        }
    }

    /// This is the main entry point, called by an InteractionTrigger.
    public void Interact()
    {
        if (isDialogueOpen) return;

        // 1. Determine the NPC's current state and relevant quest.
        UpdateCurrentAssignment();

        if (currentAssignment == null)
        {
            Debug.LogWarning("[MultiQuestNPC] This NPC has no quests assigned in its progression list.", this);
            return;
        }

        // 2. Select the correct dialogue for the current state.
        DialogueData dialogueToShow = GetCurrentDialogue();

        // 3. Start the dialogue if one is available.
        if (dialogueToShow != null)
        {
            isDialogueOpen = true;
            LockPlayer(true);
            DialogueManager.Instance.StartDialogue(dialogueToShow);
            DialogueManager.Instance.onDialogueEnd += OnDialogueClosed;
        }
    }

    /// This is the core logic. It figures out which quest the NPC should be focused on.
    private void UpdateCurrentAssignment()
    {
        currentAssignment = null;

        // Find the LATEST quest in this NPC's list that is either ACTIVE or COMPLETED.
        // This tells us where the player is in this specific NPC's storyline.
        QuestAssignment lastRelevantAssignment = questProgression
            .Where(qa => qa.quest != null && (GameManager.gameManager.activeQuests.Contains(qa.quest) || QuestSaveManager.Instance.IsQuestCompleted(qa.quest.questID)))
            .LastOrDefault();

        if (lastRelevantAssignment != null)
        {
            // We found where we are in the story.
            currentAssignment = lastRelevantAssignment;
        }
        else
        {
            // If no quests are active or completed, the NPC's job is to offer the very first one.
            if (questProgression.Count > 0)
            {
                currentAssignment = questProgression[0];
            }
        }
    }

    /// Based on the current assignment, returns the correct dialogue to display.
    private DialogueData GetCurrentDialogue()
    {
        if (currentAssignment == null) return null;

        // If the quest for our current story beat is completed, show the "after" dialogue.
        if (QuestSaveManager.Instance.IsQuestCompleted(currentAssignment.quest.questID))
        {
            return currentAssignment.dialogueAfterQuest;
        }
        else // Otherwise, the quest is either not yet given or in progress.
        {
            return currentAssignment.dialogueBeforeQuest;
        }
    }

    /// Callback function that runs when the dialogue UI is closed.
    private void OnDialogueClosed()
    {
        isDialogueOpen = false;
        LockPlayer(false);
        DialogueManager.Instance.onDialogueEnd -= OnDialogueClosed;

        if (currentAssignment != null)
        {
            // --- NAVIGATION LOGIC ---
            // Check if navigation should be triggered for this specific quest assignment.
            if (currentAssignment.enableNavigation && currentAssignment.navigationTargetIndex >= 0)
            {
                if (NavigationManager.nevigationManager != null)
                {
                    NavigationManager.nevigationManager.SetDestination(currentAssignment.navigationTargetIndex);
                    NavigationManager.nevigationManager.EnableNavigation(true);
                }
            }

            // --- QUEST LOGIC ---
            if (!QuestSaveManager.Instance.IsQuestCompleted(currentAssignment.quest.questID))
            {
                if (GameManager.gameManager.activeQuests.Contains(currentAssignment.quest))
                {
                    GameManager.gameManager.CompleteQuest(currentAssignment.quest);
                }
                else
                {
                    GameManager.gameManager.AddQuest(currentAssignment.quest);
                }
            }
        }
    }

    /// A helper function to lock or unlock player movement.
    private void LockPlayer(bool state)
    {
        if (playerController != null)
        {
            playerController.lockMovement = state;
        }

        if (playerInput != null)
        {
            playerInput.enabled = !state;
        }
    }
}