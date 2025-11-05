using UnityEngine;
using Invector.vCharacterController;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class InteractionTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject ActionHintTrigger;
    public NPCDialogueTrigger dialogueTrigger;

    [Header("Settings")]
    public float activationDelay = 1f;
    public float reinteractionCooldown = 2.5f; // seconds before you can interact again (while inside)

    private bool playerInRange = false;
    private bool triggerActive = false;
    private bool canInteract = true;
    private bool isCoolingDown = false;

    private vThirdPersonController controller;
    private vThirdPersonInput controllerInput;
    public int navigationTargetIndex = -1; //updated code to set targets of nevmash destination

    private void Start()
    {
        if (ActionHintTrigger != null)
            ActionHintTrigger.SetActive(false);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            controller = playerObj.GetComponent<vThirdPersonController>();
            controllerInput = playerObj.GetComponent<vThirdPersonInput>();
        }

        Invoke(nameof(ActivateTrigger), activationDelay);
    }

    private void ActivateTrigger() => triggerActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerActive || !other.CompareTag("Player")) return;

        playerInRange = true;

        // Show prompt only if player can currently interact and dialogue isn’t open

        if (ActionHintTrigger != null && dialogueTrigger != null)
        {
            if (canInteract && !dialogueTrigger.IsDialogueOpen)
                ActionHintTrigger?.SetActive(true);
        }
        else
        {
            if ( navigationTargetIndex >= 0)
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
    }

    private void OnTriggerExit(Collider other)
    {

        if (ActionHintTrigger != null && dialogueTrigger != null) //updated code to set targets of nevmash destination
        {
            if (!other.CompareTag("Player")) return;

            playerInRange = false;
            ActionHintTrigger?.SetActive(false);

            // reset cooldown when leaving the trigger zone
            canInteract = true;
            isCoolingDown = false;
            StopAllCoroutines();
        }
    }

    private void Update()
    {
        if (ActionHintTrigger != null && dialogueTrigger != null) //updated code to set targets of nevmash destination
        {
            if (!triggerActive || !playerInRange || dialogueTrigger.IsDialogueOpen || !canInteract)
                return;

            if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.E))
            {
                dialogueTrigger.Interact();
                ActionHintTrigger?.SetActive(false);

                // lock interaction temporarily
                StartCoroutine(LocalCooldown());
            }
        }
    }

    private IEnumerator LocalCooldown()
    {
        canInteract = false;
        isCoolingDown = true;

        float timer = 0f;
        while (timer < reinteractionCooldown && playerInRange)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // only re-enable if player is still inside zone after cooldown
        if (playerInRange)
        {
            canInteract = true;
            isCoolingDown = false;
            if (!dialogueTrigger.IsDialogueOpen)
                ActionHintTrigger?.SetActive(true);
        }
    }

    // Called externally by DialogueTrigger when conversation ends
    public void EnablePrompt()
    {
        if (playerInRange && canInteract && !dialogueTrigger.IsDialogueOpen)
            ActionHintTrigger?.SetActive(true);
    }

    public void DisablePrompt()
    {
        ActionHintTrigger?.SetActive(false);
    }
}
