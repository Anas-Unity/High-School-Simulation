using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QuestCompletionTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    [Tooltip("The quest that will be marked as 'completed' when the player enters this trigger.")]
    [SerializeField] private QuestData questToComplete;

    [Tooltip("If checked, this trigger GameObject will be disabled after it fires once.")]
    [SerializeField] private bool disableOnComplete = true;

    private void Awake()
    {
        // Ensure the collider is a trigger.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player and if the quest is valid and active.
        if (other.CompareTag("Player") && questToComplete != null && GameManager.gameManager.activeQuests.Contains(questToComplete))
        {
            Debug.Log($"[QuestCompletionTrigger] Player entered trigger, completing quest: '{questToComplete.questTitle}'.");

            // Tell the GameManager to complete the quest.
            GameManager.gameManager.CompleteQuest(questToComplete);

            if (disableOnComplete)
            {
                gameObject.SetActive(false);
            }
        }
    }
}