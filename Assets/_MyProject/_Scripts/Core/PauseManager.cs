using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The parent GameObject for your pause menu UI elements.")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Tooltip("The parent GameObject for your on-screen player controls (joystick, buttons, etc.).")]
    [SerializeField] private GameObject playerControlsUI;

    [Header("Scene Configuration")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden when the game starts.
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Keep the Escape key functionality as a backup/for PC builds.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// A public method that can be called by either a UI button or the Escape key.
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        if (playerControlsUI != null) playerControlsUI.SetActive(true); // Show controls

        Time.timeScale = 1f; // Unfreeze the game
        isPaused = false;
    }

    // Made public so the on-screen pause button can call it directly.
    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        if (playerControlsUI != null) playerControlsUI.SetActive(false); // Hide controls

        Time.timeScale = 0f; // Freeze the game
        isPaused = true;
    }

    /// <summary>
    /// This is your "Quit to Menu" function.
    /// </summary>
    public void LoadMenu()
    {
        // CRITICAL: You MUST unfreeze time before leaving the scene,
        // otherwise your main menu might start frozen.
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}