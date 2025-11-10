using UnityEngine;
using UnityEngine.Playables;
using Invector.vCharacterController;
using ControlFreak2;                  // For Control Freak 2 Input
using UnityEngine.UI;                 // For standard UI Text
using TMPro;                          // For TextMeshPro Text (Optional)

[RequireComponent(typeof(PlayableDirector))]
public class TimelineController : MonoBehaviour
{
    [Header("Core Components")]
    [Tooltip("The PlayableDirector that runs the timeline. It's assigned automatically if left empty.")]
    public PlayableDirector playableDirector;

    [Header("Playback Settings")]
    [Tooltip("If checked, the player must press the 'Submit' button (Enter/Space/A button) to start the timeline.")]
    public bool requireInput = true;

    [Tooltip("The name of the Action Button as defined in your Control Freak 2 Input Settings (e.g., 'Submit', 'Action').")]
    public string actionButtonName = "Submit";
  
    [Header("UI Hint Panel (Input Required Only)")]
    [Tooltip("(Optional) The UI Panel that will be shown when the player enters the trigger.")]
    public GameObject hintPanel;
    [Tooltip("(Optional) The custom message to display in the hint panel.")]
    [TextArea] public string hintMessage;
    [Tooltip("(Optional) Assign the legacy UI Text component for the hint message.")]
    public Text hintText;
    [Tooltip("(Optional) Assign the TextMeshPro component for the hint message.")]
    public TextMeshProUGUI hintTextTMP;

    [Header("Player Control")]
    [Tooltip("Drag the Player GameObject's 'vThirdPersonInput' component here.")]
    public vThirdPersonInput playerController;

    [Header("Scene Object & UI Control")]
    [Tooltip("(Optional) Drag any GameObject here (like your 'bus') that should be activated when the timeline starts.")]
    public GameObject objectToActivate;

    [Tooltip("(Optional) GameObject to DEACTIVATE when the timeline starts (e.g., the player model).")]
    public GameObject objectToDeactivate;
    public GameObject deactivateTrigger;

    [Tooltip("(Optional) Drag your main in-game UI canvas or panel here. It will be hidden during the cutscene.")]
    public GameObject inGameUI;

    [Header("Cleanup Action")]
    [Tooltip("If checked, this entire prefab GameObject will be disabled after the timeline finishes to ensure it only runs once.")]
    public bool disableOnFinish = true;

    [Header("Post-Timeline Actions")]
    [Tooltip("(Optional) A transform defining where the player should be moved to after the timeline finishes.")]
    public Transform playerEndPosition;

    // Internal state to track if the player is in the trigger zone
    private bool isPlayerInTrigger = false;

    void Awake()
    {
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineFinished;
            Debug.Log($"[{gameObject.name}] Awake: Initialized and subscribed to timeline 'stopped' event.");
        }
    }

    void OnDestroy()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineFinished;
        }
    }

    void Update()
    {
        if (requireInput && isPlayerInTrigger)
        {
            Debug.Log("Enterd update function waiting for 'action' button.");
            if (CF2Input.GetButtonDown(actionButtonName))
            {
                Debug.Log($"[{gameObject.name}] Update: 'Action' button pressed by player.");
                PlayTimeline();
            }
        }
    }

    public void PlayerDidEnterTrigger()
    {
        isPlayerInTrigger = true;
        Debug.Log($"[{gameObject.name}] PlayerDidEnterTrigger: Player has entered the trigger zone.");

        if (!requireInput)
        {
            Debug.Log($"[{gameObject.name}] PlayerDidEnterTrigger: 'Require Input' is false. Playing timeline automatically.");
            PlayTimeline();

            //Debug.Log($"[{gameObject.name}] PlayerDidEnterTrigger: Navigation Path Stoped.");
            //NavigationManager.nevigationManager.HideNavigation();

            // Deactivating the timeline trigger GO.
            deactivateTrigger.SetActive(false);
        }
        else
        {
            ShowHintPanel();
            Debug.Log($"[{gameObject.name}] PlayerDidEnterTrigger: 'Require Input' is true. Waiting for player to press the action button.");
        }
    }

    public void PlayerDidExitTrigger()
    {
        isPlayerInTrigger = false;
        HideHintPanel();
        Debug.Log($"[{gameObject.name}] PlayerDidExitTrigger: Player has left the trigger zone.");
    }

    private void PlayTimeline()
    {
        Debug.Log($"[{gameObject.name}] PlayTimeline: ----- STARTING CUTSCENE -----");
        HideHintPanel();
                
        // --- 1. SETUP THE SCENE FOR THE CUTSCENE ---

        // Disable player movement using Invector's built-in function
        if (playerController != null)
        {
            playerController.SetLockBasicInput(true);
            Debug.Log($"[{gameObject.name}] PlayTimeline: Called SetLockAllInput(true) on '{playerController.GetType().Name}'. Player input is now locked.");
        }

        if (inGameUI != null)
        {
            inGameUI.SetActive(false);
            Debug.Log($"[{gameObject.name}] PlayTimeline: In-game UI '{inGameUI.name}' hidden.");
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"[{gameObject.name}] PlayTimeline: GameObject '{objectToActivate.name}' activated.");
        }

        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log($"[{gameObject.name}] PlayTimeline: GameObject '{objectToDeactivate.name}' Deactivated.");
        }

        // --- 2. PLAY THE TIMELINE ---
        Debug.Log($"[{gameObject.name}] PlayTimeline: Calling Play() on PlayableDirector.");
        playableDirector.Play();

        // --- 3. PREVENT RE-TRIGGERING ---
        Collider triggerCollider = GetComponentInChildren<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }

        this.enabled = false;
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log($"[{gameObject.name}] OnTimelineFinished: ----- CUTSCENE ENDED -----");
        // --- CLEANUP ACTIONS AFTER THE CUTSCENE ---
        // If an object was deactivated (the player), re-activate it first.
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(true);
        }

        // If an end position is set, teleport the player there.
        // We use the playerController's transform to move the whole player object.
        if (playerEndPosition != null && playerController != null)
        {
            playerController.transform.position = playerEndPosition.position;
            playerController.transform.rotation = playerEndPosition.rotation;
            Debug.Log($"[{gameObject.name}] OnTimelineFinished: Teleporting Player '{playerController.GetType().Name}'. at the destination.");
        }

        // Re-enable player movement using Invector's built-in function
        if (playerController != null)
        {
            playerController.SetLockBasicInput(false);
            Debug.Log($"[{gameObject.name}] OnTimelineFinished: Called SetLockAllInput(false) on '{playerController.GetType().Name}'. Player input is now unlocked.");
        }

        if (inGameUI != null)
        {
            inGameUI.SetActive(true);
            Debug.Log($"[{gameObject.name}] OnTimelineFinished: In-game UI '{inGameUI.name}' is now visible.");
        }

        if (disableOnFinish)
        {
            this.gameObject.SetActive(false);
            Debug.Log($"[{gameObject.name}] OnTimelineFinished: 'Disable On Finish' is true. Disabling entire prefab GameObject.");
        }
    }

    private void ShowHintPanel()
    {
        if (hintPanel == null) return;

        Debug.Log($"[{gameObject.name}] ShowHintPanel: Showing hint panel.");
        // Set the custom text message
        if (!string.IsNullOrEmpty(hintMessage))
        {
            if (hintText != null) hintText.text = hintMessage;
            if (hintTextTMP != null) hintTextTMP.text = hintMessage;
            Debug.Log($"[{gameObject.name}] ShowHintPanel: Setting hint text to '{hintMessage}'.");
        }
        hintPanel.SetActive(true);
    }

    private void HideHintPanel()
    {
        if (hintPanel != null && hintPanel.activeSelf)
        {
            Debug.Log($"[{gameObject.name}] HideHintPanel: Hiding hint panel.");
            hintPanel.SetActive(false);
        }
    }
}