using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class UIFader : MonoBehaviour
{
    private Image fadeImage; // Changed from [SerializeField]


    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogError("UIFader requires an Image component.", this);
            this.enabled = false;
            return;
        }

        // Start completely transparent and non-interactive
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        //HideRaycast();
    }

    public IEnumerator FadeOut(float duration)
    {
        fadeImage.enabled = true; // Enable the image component
        fadeImage.raycastTarget = true; // It should block input while fading out
        yield return Fade(1f, duration);
    }

    public IEnumerator FadeIn(float duration)
    {
        // Don't block input while fading in
        fadeImage.raycastTarget = false;
        yield return Fade(0f, duration);

        // Once fully faded in (transparent), disable the component again
        fadeImage.enabled = false;
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }
    public void HideRaycast()
    {
        fadeImage.raycastTarget = false;
        fadeImage.enabled = false;       
    }
    private void SetAlpha(float a)
    {
        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}