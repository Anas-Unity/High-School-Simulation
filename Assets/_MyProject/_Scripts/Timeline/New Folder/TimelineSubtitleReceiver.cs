using UnityEngine;
using UnityEngine.Playables;

public class TimelineSubtitleReceiver : MonoBehaviour, INotificationReceiver
{
    [Header("References")]
    public SubtitleUI subtitleUI;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is TimelineSubtitleMarker marker)
        {
            if (subtitleUI != null)
            {
                subtitleUI.ShowSubtitle(marker.subtitleText, marker.displayDuration);
                Debug.Log($"🎞️ Subtitle Triggered: {marker.subtitleText}");
            }
        }
    }
}
