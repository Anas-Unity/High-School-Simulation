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

    // 🔹 Optional: assign this to control what navigation target index to set after this dialogue
    [Header("Navigation Settings")]
    public bool enableNavigationAfterDialogue = false;
    public int navigationTargetIndex = -1; // e.g. 0 = School, 1 = Market

    private void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

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
