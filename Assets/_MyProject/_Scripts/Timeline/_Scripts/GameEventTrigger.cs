using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    [Header("Event Settings")]
    [Tooltip("The unique key of the timeline to be played (must match a key in TimelineManager).")]
    public string timelineKey;
    [Tooltip("If checked, the timeline will be triggered automatically when the scene starts.")]
    public bool playOnStart = false;

    [Header("Persistence")]
    [Tooltip("If checked, this trigger will use PlayerPrefs to ensure it only activates ONCE per game.")]
    public bool playOnlyOnce = false;

    [Header("Optional References")]
    [Tooltip("(Optional) Assign the UIFader here to hide its raycast when skipping a 'Play Once' event on game start.")]
    public UIFader fader;


    private void Start()
    {
        if (playOnlyOnce && !string.IsNullOrEmpty(timelineKey))
        {
            if (PlayerPrefs.GetInt(timelineKey, 0) == 1)
            {
                Debug.Log($"GameEventTrigger for '{timelineKey}' has already been played. Disabling this trigger.");

                // If this trigger is being skipped and it has a reference to the fader,
                // tell the fader to hide its raycast blocker immediately.
                if (fader != null)
                {
                    fader.HideRaycast();
                    Debug.Log("UIFader raycast hidden because the timeline was skipped on start.");
                }

                gameObject.SetActive(false);
                return;
            }
        }

        if (playOnStart)
        {
            TriggerTimeline();
        }
    }

    public void TriggerTimeline()
    {
        if (playOnlyOnce)
        {
            if (PlayerPrefs.GetInt(timelineKey, 0) == 1)
            {
                return;
            }
            if (string.IsNullOrEmpty(timelineKey))
            {
                Debug.LogError("'Play Only Once' is checked, but 'Timeline Key' is empty! Cannot save.", this);
                return;
            }
            PlayerPrefs.SetInt(timelineKey, 1);
            PlayerPrefs.Save();
            Debug.Log($"Saved progress: Trigger for '{timelineKey}' marked as played permanently.");
        }

        Debug.Log($"GameEventTrigger: Requesting TimelineManager to play timeline with key: '{timelineKey}'");
        if (TimelineManager.Instance == null)
        {
            Debug.LogError("TimelineManager not found in scene. Cannot play timeline.", this);
            return;
        }

        TimelineManager.Instance.PlayTimelineByKey(timelineKey);
    }
}