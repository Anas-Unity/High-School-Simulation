using UnityEngine;
using Invector.vCharacterController;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class InteractionTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The UI element that shows the 'Press E' prompt.")]
    public GameObject ActionHintTrigger;

    [Tooltip("Drag the component you want to interact with here (e.g., MultiQuestNPC, NPCDialogueTrigger).")]
    public MonoBehaviour interactableTarget;
    private Interactable interactable; // The private reference to the interface

    [Header("Settings")]
    public float activationDelay = 1f;
    public float reinteractionCooldown = 2.5f;

    // --- Private state variables ---
    private bool playerInRange = false;
    private bool triggerActive = false;
    private bool canInteract = true;

    private void Start()
    {
        // 1. Get the Interactable interface from the component you dragged into the Inspector.
        // Notice the capital 'I'. This must match your interface file name exactly.
        interactable = interactableTarget as Interactable;

        // 2. Validate the setup. If it's wrong, stop immediately.
        if (interactable == null)
        {
            Debug.LogError($"FATAL ERROR: The component assigned to 'Interactable Target' on '{this.gameObject.name}' does not implement the IInteractable interface! Check for typos or missing scripts.", this.gameObject);
            this.enabled = false;
            return;
        }

        // 3. Hide the UI prompt at the start.
        if (ActionHintTrigger != null)
            ActionHintTrigger.SetActive(false);

        // 4. Schedule the trigger to become active.
        Invoke(nameof(ActivateTrigger), activationDelay);
    }

    private void ActivateTrigger() => triggerActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerActive || !other.CompareTag("Player") || interactable == null) return;

        playerInRange = true;

        // We now check the interface property, not the old hard-coded reference.
        if (canInteract && !interactable.IsDialogueOpen)
        {
            ActionHintTrigger?.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        ActionHintTrigger?.SetActive(false);

        // Reset cooldown when leaving the trigger zone
        canInteract = true;
        StopAllCoroutines();
    }

    private void Update()
    {
        // This is the new, robust check. It ensures all conditions are met before listening for input.
        if (triggerActive && playerInRange && canInteract && !interactable.IsDialogueOpen)
        {
            if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact(); // Call the Interact method on our target.
                ActionHintTrigger?.SetActive(false);
                StartCoroutine(LocalCooldown());
            }
        }
    }

    private IEnumerator LocalCooldown()
    {
        canInteract = false;
        float timer = 0f;
        while (timer < reinteractionCooldown && playerInRange)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (playerInRange)
        {
            canInteract = true;
            // Check the interface property again before showing the prompt.
            if (!interactable.IsDialogueOpen)
            {
                ActionHintTrigger?.SetActive(true);
            }
        }
    }
}