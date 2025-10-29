using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [Header("Quest Info")]
    [SerializeField] private QuestData questToGive;
    [SerializeField] private bool giveOnFirstDialogue = false;

    [Header("NPC Info")]
    [SerializeField] private string npcName = "Unknown"; // Set this per-NPC in Inspector

    public void GiveQuest()
    {
        if (questToGive == null)
        {
            Debug.LogWarning("[QuestGiver] No quest assigned!");
            return;
        }

        // Assign who gave the quest (for QuestBook display)
        questToGive.questGiverName = npcName;

        // Add quest to GameManager
        GameManager.gameManager.AddQuest(questToGive);

        Debug.Log($"[QuestGiver] '{npcName}' gave quest '{questToGive.questTitle}'");
    }

    public void CompleteQuest()
    {
        if (questToGive == null)
        {
            Debug.LogWarning("[QuestGiver] No quest assigned to complete!");
            return;
        }

        GameManager.gameManager.CompleteQuest(questToGive);
        Debug.Log($"[QuestGiver] Quest '{questToGive.questTitle}' completed by player");
    }

    // Call this automatically from DialogueManager after first dialogue ends (if flagged)
    public void OnDialogueEnd()
    {
        if (giveOnFirstDialogue)
            GiveQuest();
    }
}
