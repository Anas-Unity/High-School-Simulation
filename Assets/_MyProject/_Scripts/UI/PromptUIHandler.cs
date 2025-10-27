using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PromptUIHandler : MonoBehaviour
{
    private CanvasGroup cg;
    private float targetAlpha = 0f;
    public float fadeSpeed = 5f;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
    }

    void Update()
    {
        cg.alpha = Mathf.Lerp(cg.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
    }

    public void ShowPrompt()
    {
        targetAlpha = 1f;
    }

    public void HidePrompt()
    {
        targetAlpha = 0f;
    }
}
