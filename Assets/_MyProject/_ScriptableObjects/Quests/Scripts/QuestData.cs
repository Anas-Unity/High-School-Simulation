using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/New Quest")]
public class QuestData : ScriptableObject
{
    [Header("Quest ID")]
    [Tooltip("A unique ID for this quest (e.g., 'Q01_TalkToRabbit'). Cannot be changed after release!")]
    public string questID;

    [Header("Basic Info")]
    public string questTitle;
    [TextArea(2, 5)] public string questDescription;

    [Header("Quest Giver Info")]
    public string questGiverName;

    [Header("Quest Chain")]
    [Tooltip("The quest that should automatically activate when this one is completed.")]
    public QuestData nextQuest; // Link to the next quest in the sequence
}