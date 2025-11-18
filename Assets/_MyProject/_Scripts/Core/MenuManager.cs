// In MenuManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ResetProgress()
    {
        // --- THIS IS THE MODIFIED, CORRECT RESET LOGIC ---

        // 1. Erase the saved data from the disk.
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs (saved data on disk) have been deleted.");

        // 2. Command the live QuestSaveManager to purge its in-memory data.
        if (QuestSaveManager.Instance != null)
        {
            QuestSaveManager.Instance.ClearDataInMemory();
        }

        // 3. Command the live GameManager to purge its in-memory quest lists.
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.ResetQuestState();
        }

        Debug.Log("Full game progress reset has been completed.");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting application...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}