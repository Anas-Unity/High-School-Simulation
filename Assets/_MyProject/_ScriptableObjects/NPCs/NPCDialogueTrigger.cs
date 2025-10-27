using UnityEngine;
using Invector.vCharacterController;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("UI References")]
    public GameObject promptUI;
    public GameObject dialoguePanel;

    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    [Header("Time Progression")]
    public TimeOfDay timeAfterDialogue = TimeOfDay.Morning;

    [HideInInspector] public bool IsDialogueOpen { get; private set; }

    private InteractionTrigger interactionTrigger;
    private vThirdPersonController controller;
    private vThirdPersonInput controllerInput;
    private Rigidbody playerRB;

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

        // disable prompt while talking
        interactionTrigger?.DisablePrompt();

        DialogueManager.Instance.onDialogueEnd += CloseDialogue;
    }

    public void CloseDialogue()
    {
        IsDialogueOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        LockPlayer(false);

        // Apply time progression from NPC prefab
        TimeManager.Instance.SetTime(timeAfterDialogue);

        DialogueManager.Instance.onDialogueEnd -= CloseDialogue;

        interactionTrigger?.EnablePrompt();
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
