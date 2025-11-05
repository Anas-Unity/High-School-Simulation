using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SubtitleUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI subtitleText;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    [Tooltip("Fade duration for showing/hiding subtitles.")]
    public float fadeDuration = 0.4f;
    [Tooltip("Default duration each subtitle remains visible if not overridden.")]
    public float defaultDisplayTime = 3f;
    [Tooltip("Clear any current subtitle when a new one is shown.")]
    public bool clearOnNewSubtitle = true;

    private Coroutine fadeRoutine;
    private Coroutine hideRoutine;
    private Queue<(string text, float duration)> subtitleQueue = new Queue<(string, float)>();
    private bool isDisplaying;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0;

        if (subtitleText != null)
            subtitleText.text = string.Empty;
    }

    /// <summary>
    /// Enqueue a subtitle line. If nothing is playing, show it immediately.
    /// </summary>
    public void ShowSubtitle(string text, float duration = -1f)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (duration <= 0)
            duration = defaultDisplayTime;

        subtitleQueue.Enqueue((text, duration));

        if (!isDisplaying)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isDisplaying = true;

        while (subtitleQueue.Count > 0)
        {
            var (text, duration) = subtitleQueue.Dequeue();

            if (clearOnNewSubtitle)
                subtitleText.text = string.Empty;

            yield return FadeCanvas(1f);

            subtitleText.text = text;

            if (hideRoutine != null) StopCoroutine(hideRoutine);
            hideRoutine = StartCoroutine(HideAfterDelay(duration));

            // Wait until subtitle fully hidden before continuing
            yield return new WaitUntil(() => canvasGroup.alpha <= 0.01f);
        }

        isDisplaying = false;
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return FadeCanvas(0f);
    }

    private IEnumerator FadeCanvas(float target)
    {
        if (canvasGroup == null)
            yield break;

        float start = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;

        // Clear text when faded out
        if (target == 0f && subtitleText != null)
            subtitleText.text = string.Empty;
    }

    /// <summary>
    /// Immediately hides subtitles and clears queue (used when timeline is skipped or ended).
    /// </summary>
    public void HideImmediate()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        if (hideRoutine != null) StopCoroutine(hideRoutine);

        subtitleQueue.Clear();
        isDisplaying = false;

        if (canvasGroup != null)
            canvasGroup.alpha = 0;

        if (subtitleText != null)
            subtitleText.text = string.Empty;
    }
}
