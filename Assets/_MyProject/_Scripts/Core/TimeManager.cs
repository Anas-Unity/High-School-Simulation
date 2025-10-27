using UnityEngine;
using System;

public enum TimeOfDay
{
    Morning,
    Noon,
    Afternoon,
    Evening,
    Night
}

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Current Time")]
    public TimeOfDay currentTime;

    [Header("Ambient Colors")]
    public Color morningColor = new Color(1f, 0.95f, 0.85f);
    public Color noonColor = new Color(1f, 1f, 0.9f);
    public Color afternoonColor = new Color(1f, 0.85f, 0.7f);
    public Color eveningColor = new Color(0.9f, 0.5f, 0.3f);
    public Color nightColor = new Color(0.2f, 0.3f, 0.6f);

    public event Action<TimeOfDay> OnTimeChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetTime(TimeOfDay newTime)
    {
        currentTime = newTime;
        Debug.Log("Time updated to: " + newTime);

        UpdateEnvironment(newTime);
        OnTimeChanged?.Invoke(currentTime);
    }

    private void UpdateEnvironment(TimeOfDay time)
    {
        Color targetColor = morningColor;

        switch (time)
        {
            case TimeOfDay.Morning:
                targetColor = morningColor;
                break;
            case TimeOfDay.Noon:
                targetColor = noonColor;
                break;
            case TimeOfDay.Afternoon:
                targetColor = afternoonColor;
                break;
            case TimeOfDay.Evening:
                targetColor = eveningColor;
                break;
            case TimeOfDay.Night:
                targetColor = nightColor;
                break;
        }

        // Update Unity's Environment Lighting (Ambient Color)
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = targetColor;

        // Optional: if using Skybox and directional light, adjust intensity here as well
        if (RenderSettings.sun != null)
        {
            Light sun = RenderSettings.sun;
            sun.color = targetColor;
            sun.intensity = (time == TimeOfDay.Noon) ? 1.2f : 1f; // tweak as needed
        }
    }
}
