using UnityEngine;

public interface Interactable
{
    void Interact();
    bool IsDialogueOpen { get; }
}
