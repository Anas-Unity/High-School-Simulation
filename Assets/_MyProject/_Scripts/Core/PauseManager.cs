using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu is hidden when the game starts.
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Listen for the "Escape" key to toggle the pause menu.
        if (Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // <-- CRITICAL: Unfreezes the game.
        isPaused = false;
        // You may also want to unlock the cursor and re-enable player input here.
    }

    void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // <-- CRITICAL: Freezes the game.
        isPaused = true;
        // You may also want to lock the cursor and disable player input here.
    }

    public void LoadMenu()
    {
        // CRITICAL: You MUST unfreeze time before leaving the scene.
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}