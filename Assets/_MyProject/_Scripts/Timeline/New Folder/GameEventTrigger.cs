using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    [Header("Event Settings")]
    public string timelineKey;     // e.g. "StarterGuide", "BusRide"
    public bool playOnStart = false;

    private void Start()
    {
        if (playOnStart)
            TriggerTimeline();
    }

    public void TriggerTimeline()
    {
        print("Game Timeline Trigger Event is Called");
        if (TimelineManager.Instance == null)
        {
            Debug.LogError("TimelineManager not found in scene.");
            return;
        }

        TimelineManager.Instance.PlayTimelineByKey(timelineKey);
    }
}
