using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using ControlFreak2;

[System.Serializable]
public class NamedTimeline
{
    public string key;
    public PlayableAsset timelineAsset;
    [Tooltip("If true, the screen will fade out and in when this timeline starts.")]
    public bool useFader = true;
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
    private Coroutine waitForInputActionCoroutine;
    
    [Tooltip("This variable will remember if the currently playing timeline should use the fader or not.")]
    private bool _currentTimelineUsesFader = true;

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

        BuildLookup();

        if (skipButton != null)
            skipButton.SetActive(false);
    }

    private void Start()
    {
            Debug.Log("▶️ Playing StarterGuide for first-time player...");
            PlayTimelineByKey("StarterGuide");
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
        Debug.Log("PlayTimelineByKey is called");
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
        Debug.Log("Playtimeline Function is working");
        if (isPlaying || timeline == null || director == null) return;
        StartCoroutine(PlayTimelineRoutine(timeline));
    }

    private IEnumerator PlayTimelineRoutine(PlayableAsset timeline)
    {
        isPlaying = true;
        Debug.Log("isplaying variable of Timeline is getting true value ");
        string currentKey = GetTimelineKey(timeline);

       
        NamedTimeline currentTimelineSettings = GetNamedTimeline(timeline);
        // Remember this timeline's fader setting for later (when it ends/is skipped)
        _currentTimelineUsesFader = (currentTimelineSettings != null) ? currentTimelineSettings.useFader : true;


        // Now, we check the flag before fading.
        if (_currentTimelineUsesFader && fader != null)
        {
            yield return fader.FadeOut(fadeDuration);
        }

        yield return new WaitForSeconds(0.2f);

        director.playableAsset = timeline;
        director.time = 0;
        director.Play();

        // Check the flag again for fading back in.
        if (_currentTimelineUsesFader && fader != null)
        {
            yield return fader.FadeIn(fadeDuration);
        }

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
        dir.stopped -= OnTimelineFinished;
        EndCurrentTimeline();
    }

    public void EndCurrentTimeline()
    {

        Debug.Log("endcurrenttimeline being called");
        if (!isPlaying) return;
        isPlaying = false;
        Debug.Log("endcurrenttimeline being called");

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
        
        fader.HideRaycast();

        StartCoroutine(EndSequenceFade());
    }

    private IEnumerator EndSequenceFade()
    {
        // --- MODIFIED SECTION ---
        // This routine now also respects the fader setting of the timeline that just ended.
        if (_currentTimelineUsesFader && fader != null)
        {
            yield return fader.FadeOut(fadeDuration);
            yield return new WaitForSeconds(0.2f);
            yield return fader.FadeIn(fadeDuration);
        }
        // --- END MODIFICATION ---

        Debug.Log("🎬 Timeline finished or skipped.");
    }

    public void PauseTimelineForInput(string inputActionName)
    {
        if (director.state != PlayState.Playing) return;

        Debug.Log($"⏸️ Timeline paused. Waiting for input: '{inputActionName}'...");
        director.Pause();
        waitForInputActionCoroutine = StartCoroutine(WaitForInputActionRoutine(inputActionName));
    }

    private IEnumerator WaitForInputActionRoutine(string inputActionName)
    {
        while (!CF2Input.GetButtonDown(inputActionName))
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

    private NamedTimeline GetNamedTimeline(PlayableAsset asset)
    {
        foreach (var t in timelines)
        {
            if (t.timelineAsset == asset)
                return t;
        }
        return null;
    }
}