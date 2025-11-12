using UnityEngine;
using UnityEngine.SceneManagement; // <-- You MUST add this line to manage scenes.

public class MenuManager : MonoBehaviour
{
    [Header("Game Scene Referance")]
    [Tooltip("Enter the name of your game scene correctly")]
    // Assign the name of your main game scene in the Inspector.
    [SerializeField] private string gameSceneName = "GameScene";

    public void StartGame()
    {
        // Load the main game scene.
        SceneManager.LoadScene(gameSceneName);
    }

    public void ResetProgress()
    {
        // WARNING: This deletes ALL saved data.
        // This is where your QuestSaveManager stores its data.
        PlayerPrefs.DeleteAll();
        Debug.Log("Player progress has been reset.");
        // You might want to add a confirmation pop-up UI for this later.
    }

    public void QuitGame()
    {
        Debug.Log("Quitting application...");
        Application.Quit();

        // This line is for testing in the Unity Editor.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}