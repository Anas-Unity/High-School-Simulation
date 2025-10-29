using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("Basic Info")]
    public string questTitle;
    [TextArea(2, 5)] public string questDescription;

    [Header("Quest Giver Info")]
    public string questGiverName;  // e.g., "Rabbit", "Teacher"

    [Header("Quest State")]
    public bool isCompleted = false;
}
