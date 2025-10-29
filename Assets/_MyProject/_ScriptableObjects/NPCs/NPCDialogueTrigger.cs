using UnityEngine;
using Invector.vCharacterController;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject promptUI;
    public GameObject dialoguePanel;

    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    [Header("Quest Logic")]
    public QuestData questToGive;
    public QuestData questToComplete;
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

    public void Interact()
    {
        if (IsDialogueOpen || dialogueData == null)
            return;

        // Start Dialogue
        DialogueManager.Instance.StartDialogue(dialogueData);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        IsDialogueOpen = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        LockPlayer(true);

        interactionTrigger?.DisablePrompt();

        DialogueManager.Instance.onDialogueEnd += CloseDialogue;
    }

    public void CloseDialogue()
    {
        IsDialogueOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        LockPlayer(false);

        DialogueManager.Instance.onDialogueEnd -= CloseDialogue;
        interactionTrigger?.EnablePrompt();

        // --- QUEST LOGIC ---
        if (questToComplete != null)
            GameManager.gameManager.CompleteQuest(questToComplete);

        if (questToGive != null)
            GameManager.gameManager.AddQuest(questToGive);

        // --- NAVIGATION TRIGGER LOGIC ---
        if (enableNavigationAfterDialogue && navigationTargetIndex >= 0)
        {
            if (NavigationManager.nevigationManager != null)
            {
                NavigationManager.nevigationManager.SetDestination(navigationTargetIndex);

                // Automatically enable navigation after dialogue ends
                NavigationManager.nevigationManager.EnableNavigation(true);

                Debug.Log($"🧭 Navigation activated towards target index {navigationTargetIndex}");
            }
            else
            {
                Debug.LogWarning("⚠️ NavigationManager not found in scene.");
            }
        }
    }

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
