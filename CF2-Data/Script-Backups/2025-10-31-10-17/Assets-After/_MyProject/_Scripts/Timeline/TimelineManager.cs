using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

// NOTE: This script assumes you have a SaveManager class like this:
// public class SaveManager
// {
//     public static SaveManager Instance;
//     public bool HasTimelinePlayed(string key) { /* ... your logic ... */ return false; }
//     public void MarkTimelineAsPlayed(string key) { /* ... your logic ... */ }
// }


[System.Serializable]
public class NamedTimeline
{
    public string key;
    public PlayableAsset timelineAsset;
}

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager Instance;

    [Header("References")]
    public PlayableDirector director;
    public UIFader fader;
    public SubtitleUI subtitleUI;
    public GameObject skipButton;

    [Header("Settings")]
    public float fadeDuration = 1f;

    [Header("Timelines")]
    public List<NamedTimeline> timelines = new List<NamedTimeline>();

    private bool isPlaying;
    private Dictionary<string, PlayableAsset> timelineLookup;
    private TimelineSubtitleReceiver subtitleReceiver;

    // Coroutine to keep track of the input waiting process
    private Coroutine waitForInputActionCoroutine;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (director == null) director = FindObjectOfType<PlayableDirector>();
        if (fader == null) fader = FindObjectOfType<UIFader>();
        if (subtitleUI == null) subtitleUI = FindObjectOfType<SubtitleUI>();
        if (skipButton == null)
        {
            GameObject btn = GameObject.Find("SkipButton");
            if (btn != null) skipButton = btn;
        }

        subtitleReceiver = FindObjectOfType<TimelineSubtitleReceiver>();
        if (subtitleReceiver == null)
        {
            GameObject receiverGO = new GameObject("TimelineSubtitleReceiver");
            subtitleReceiver = receiverGO.AddComponent<TimelineSubtitleReceiver>();
            subtitleReceiver.subtitleUI = subtitleUI;
        }

        BuildLookup();

        if (skipButton != null)
            skipButton.SetActive(false);
    }

    private void Start()
    {
        // Auto-play StarterGuide ONLY if it's the first time
        if (SaveManager.Instance != null && !SaveManager.Instance.HasTimelinePlayed("StarterGuide"))
        {
            Debug.Log("▶️ Playing StarterGuide for first-time player...");
            PlayTimelineByKey("StarterGuide");
        }
    }

    public void BuildLookup()
    {
        timelineLookup = new Dictionary<string, PlayableAsset>();
        foreach (var t in timelines)
        {
            if (!string.IsNullOrEmpty(t.key) && t.timelineAsset != null)
            {
                if (!timelineLookup.ContainsKey(t.key))
                    timelineLookup[t.key] = t.timelineAsset;
                else
                    Debug.LogWarning($"⚠️ Duplicate timeline key detected: {t.key}");
            }
        }
    }

    public void PlayTimelineByKey(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (!timelineLookup.ContainsKey(key))
        {
            Debug.LogWarning($"⚠️ Timeline key '{key}' not found in list.");
            return;
        }
        PlayTimeline(timelineLookup[key]);
    }

    public void PlayTimeline(PlayableAsset timeline)
    {
        if (isPlaying || timeline == null || director == null) return;
        StartCoroutine(PlayTimelineRoutine(timeline));
    }

    private IEnumerator PlayTimelineRoutine(PlayableAsset timeline)
    {
        isPlaying = true;

        string currentKey = GetTimelineKey(timeline);

        // Skip if this timeline has already been completed
        if (SaveManager.Instance != null && !string.IsNullOrEmpty(currentKey))
        {
            if (SaveManager.Instance.HasTimelinePlayed(currentKey))
            {
                Debug.Log($"⏩ Skipping already played timeline: {currentKey}");
                isPlaying = false;
                yield break;
            }
        }

        if (fader != null) yield return fader.FadeOut(fadeDuration);
        yield return new WaitForSeconds(0.2f);

        director.playableAsset = timeline;
        director.time = 0;
        director.Play();

        if (fader != null) yield return fader.FadeIn(fadeDuration);

        if (skipButton != null)
        {
            skipButton.SetActive(true);
            Button btn = skipButton.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(EndCurrentTimeline);
        }

        director.stopped += OnTimelineFinished;
    }

    private string GetTimelineKey(PlayableAsset asset)
    {
        foreach (var kvp in timelineLookup)
        {
            if (kvp.Value == asset) return kvp.Key;
        }
        return null;
    }

    public void ShowSubtitle(string text, float duration = 3f)
    {
        if (subtitleUI != null) subtitleUI.ShowSubtitle(text, duration);
    }

    private void OnTimelineFinished(PlayableDirector dir)
    {
        // Make sure to unbind the event so it doesn't get called multiple times
        dir.stopped -= OnTimelineFinished;
        EndCurrentTimeline();
    }

    public void EndCurrentTimeline()
    {
        if (!isPlaying) return;
        isPlaying = false;

        // If we are waiting for input, stop that process
        if (waitForInputActionCoroutine != null)
        {
            StopCoroutine(waitForInputActionCoroutine);
            waitForInputActionCoroutine = null;
        }

        if (director != null && director.state == PlayState.Playing)
            director.Stop();

        if (skipButton != null)
            skipButton.SetActive(false);

        if (subtitleUI != null)
            subtitleUI.HideImmediate();

        // Save timeline progress
        if (SaveManager.Instance != null && director.playableAsset != null)
        {
            string key = GetTimelineKey(director.playableAsset);
            if (!string.IsNullOrEmpty(key))
            {
                SaveManager.Instance.MarkTimelineAsPlayed(key);
                Debug.Log($"💾 Timeline progress saved: {key}");
            }
        }

        StartCoroutine(EndSequenceFade());
    }

    private IEnumerator EndSequenceFade()
    {
        if (fader != null)
        {
            yield return fader.FadeOut(fadeDuration);
            yield return new WaitForSeconds(0.2f);
            yield return fader.FadeIn(fadeDuration);
        }
        Debug.Log("🎬 Timeline finished or skipped.");
    }

    // --- NEW HYBRID FUNCTIONALITY ---

    public void PauseTimelineForInput(string inputActionName)
    {
        if (director.state != PlayState.Playing) return;

        Debug.Log($"⏸️ Timeline paused. Waiting for input: '{inputActionName}'...");
        director.Pause();
        waitForInputActionCoroutine = StartCoroutine(WaitForInputActionRoutine(inputActionName));
    }

    private IEnumerator WaitForInputActionRoutine(string inputActionName)
    {
        while (!ControlFreak2.CF2Input.GetButtonDown(inputActionName))
        {
            yield return null;
        }
        Debug.Log($"▶️ Input '{inputActionName}' detected! Resuming timeline.");
        ResumeTimeline();
    }

    private void ResumeTimeline()
    {
        waitForInputActionCoroutine = null;
        if (director != null)
        {
            director.Resume();
        }
    }
}