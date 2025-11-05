using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class TimelineSubtitleMarker : Marker, INotification
{
    [TextArea]
    public string subtitleText;

    public float displayDuration = 3f;

    // Required by INotification
    public PropertyName id => new PropertyName();
}
