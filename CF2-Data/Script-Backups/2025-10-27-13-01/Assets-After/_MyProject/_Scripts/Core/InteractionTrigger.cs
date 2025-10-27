/*using UnityEngine;
using Invector.vCharacterController;

[RequireComponent(typeof(Collider))]
public class InteractionTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject promptUI;
    public NPCDialogueTrigger dialogueTrigger;

    [Header("Settings")]
    public float activationDelay = 1f;

    private bool playerInRange = false;
    private bool triggerActive = false;
    private bool canShowPrompt = true;

    private vThirdPersonController controller;
    private vThirdPersonInput controllerInput;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            controller = playerObj.GetComponent<vThirdPersonController>();
            controllerInput = playerObj.GetComponent<vThirdPersonInput>();
        }

        // Small delay to avoid instant trigger detection at game start
        Invoke(nameof(ActivateTrigger), activationDelay);
    }

    private void ActivateTrigger() => triggerActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerActive || !other.CompareTag("Player")) return;

        playerInRange = true;
        if (canShowPrompt && !dialogueTrigger.IsDialogueOpen)
            promptUI?.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        promptUI?.SetActive(false);
        canShowPrompt = true; // reset when leaving
    }

    private void Update()
    {
        if (!triggerActive || !playerInRange || dialogueTrigger.IsDialogueOpen)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            promptUI?.SetActive(false);
            canShowPrompt = false;
            dialogueTrigger.Interact();
        }
    }

    // Called externally by NPCDialogueTrigger when dialogue closes
    public void EnablePrompt()
    {
        canShowPrompt = true;
        if (playerInRange && !dialogueTrigger.IsDialogueOpen)
            promptUI?.SetActive(true);
    }

    public void DisablePrompt()
    {
        canShowPrompt = false;
        promptUI?.SetActive(false);
    }
}
*/

using UnityEngine;
using Invector.vCharacterController;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class InteractionTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject promptUI;
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

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);

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
        if (canInteract && !dialogueTrigger.IsDialogueOpen)
            promptUI?.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        promptUI?.SetActive(false);

        // reset cooldown when leaving the trigger zone
        canInteract = true;
        isCoolingDown = false;
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!triggerActive || !playerInRange || dialogueTrigger.IsDialogueOpen || !canInteract)
            return;

        if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.E))
        {
            dialogueTrigger.Interact();
            promptUI?.SetActive(false);

            // lock interaction temporarily
            StartCoroutine(LocalCooldown());
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
                promptUI?.SetActive(true);
        }
    }

    // Called externally by DialogueTrigger when conversation ends
    public void EnablePrompt()
    {
        if (playerInRange && canInteract && !dialogueTrigger.IsDialogueOpen)
            promptUI?.SetActive(true);
    }

    public void DisablePrompt()
    {
        promptUI?.SetActive(false);
    }
}
