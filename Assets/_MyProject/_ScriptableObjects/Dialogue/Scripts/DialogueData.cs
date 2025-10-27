using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueData : ScriptableObject
{
    [Header("Dialogue Info")]
    public string characterName;
    [TextArea(3, 7)]
    public string[] dialogueLines;

    [Header("Button Settings")]
    public bool useTwoButtons = false;
    public string primaryButtonText = "Continue";
    public string secondaryButtonText = "Cancel";
}
