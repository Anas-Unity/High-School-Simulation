using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TimelineTriggerVolume : MonoBehaviour
{
    [Tooltip("The tag of the object that can activate this trigger (e.g., 'Player').")]
    public string triggerTag = "Player";

    // A reference to the main controller on the parent
    private TimelineController timelineController;

    void Awake()
    {
        // Find the controller on our parent object
        timelineController = GetComponentInParent<TimelineController>();

        if (timelineController == null)
        {
            Debug.LogError("TimelineTriggerVolume could not find a TimelineController on its parent!", this);
            this.enabled = false; // Disable script if setup is wrong
        }

        // Ensure the collider is actually a trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the player enters, tell the parent controller
        if (other.CompareTag(triggerTag))
        {
            timelineController.PlayerDidEnterTrigger();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the player exits, tell the parent controller
        if (other.CompareTag(triggerTag))
        {
            timelineController.PlayerDidExitTrigger();
        }
    }
}