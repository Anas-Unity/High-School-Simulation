using UnityEngine;

/// <summary>
/// Receives a signal from Timeline and tells the TimelineManager to pause
/// and wait for a specific player input before resuming.
/// </summary>
public class InteractionSignalProxy : MonoBehaviour
{
    [Tooltip("The name of the input action to wait for (e.g., 'Jump', 'Fire1'). Must match the name in Unity's Input Manager.")]
    public string inputActionName;

    /// <summary>
    /// This function is called by the Signal Emitter in Timeline.
    /// </summary>
    public void Trigger()
    {
        if (TimelineManager.Instance == null)
        {
            Debug.LogError("TimelineManager not found. Cannot pause for input.");
            return;
        }

        if (string.IsNullOrEmpty(inputActionName))
        {
            Debug.LogWarning("InputActionName is not set on this proxy. Cannot wait for input.", this);
            return;
        }

        // Tell the TimelineManager to handle the pause and wait logic
        TimelineManager.Instance.PauseTimelineForInput(inputActionName);
    }
}