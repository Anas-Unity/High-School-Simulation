using UnityEngine;

/// <summary>
/// Attach one of these to a GameObject in scene (or create prefab).
/// In the Timeline SignalReceiver, bind the signal to the Trigger() method.
/// Set the 'subtitleText' and 'displayDuration' in Inspector for each proxy.
/// </summary>
public class SubtitleSignalProxy : MonoBehaviour
{
    [TextArea]
    public string subtitleText = "Subtitle";
    public float displayDuration = 3f;

    public void Trigger()
    {
        if (TimelineManager.Instance != null)
            TimelineManager.Instance.ShowSubtitle(subtitleText, displayDuration);
        else
            Debug.LogWarning("TimelineManager not found to show subtitle.");
    }
}
