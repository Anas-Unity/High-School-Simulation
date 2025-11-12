using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A dedicated listener that performs actions on GameObjects when a specific quest is completed.
/// Attach this to a persistent manager object in your scene.
/// </summary>
public class QuestEventListener : MonoBehaviour
{
    [Header("Listener Settings")]
    [Tooltip("The quest this component is listening for.")]
    [SerializeField] private QuestData questToListenFor;

    [Header("Actions on Completion")]
    [Tooltip("GameObjects to ACTIVATE when the quest is completed.")]
    [SerializeField] private List<GameObject> objectsToActivate;

    [Tooltip("GameObjects to DEACTIVATE when the quest is completed.")]
    [SerializeField] private List<GameObject> objectsToDeactivate;

    private void OnEnable()
    {
        // Subscribe to the GameManager's event when this component becomes active.
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.OnQuestCompleted += OnQuestCompletedHandler;
        }
        else
        {
            Debug.LogError("[QuestEventListener] GameManager not found! This component requires a GameManager in the scene.");
        }
    }

    private void OnDisable()
    {
        // IMPORTANT: Unsubscribe when the component is disabled or destroyed to prevent memory leaks.
        if (GameManager.gameManager != null)
        {
            GameManager.gameManager.OnQuestCompleted -= OnQuestCompletedHandler;
        }
    }

    /// <summary>
    /// This method is called by the GameManager's event whenever ANY quest is completed.
    /// </summary>
    /// <param name="completedQuest">The QuestData asset of the quest that was just completed.</param>
    private void OnQuestCompletedHandler(QuestData completedQuest)
    {
        // Check if the completed quest is the specific one we are listening for.
        if (completedQuest == questToListenFor)
        {
            Debug.Log($"[QuestEventListener] Detected completion of '{completedQuest.questTitle}'. Performing actions.");

            // Perform the actions.
            ActivateObjects();
            DeactivateObjects();

            // Optional: Destroy this listener component after it has fired to prevent it from running again.
            // Destroy(this);
        }
    }

    private void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void DeactivateObjects()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}