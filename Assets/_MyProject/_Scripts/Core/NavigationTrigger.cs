using UnityEngine;

/// <summary>
/// A simple, automatic trigger that activates a navigation path when the player enters
/// and deactivates it when they leave.
/// Its only job is to talk to the NavigationManager.
/// </summary>
[RequireComponent(typeof(Collider))]
public class NavigationTrigger : MonoBehaviour
{
    [Header("Navigation Settings")]
    [Tooltip("The index of the destination in the NavigationManager's list that this trigger should point to.")]
    [SerializeField] private int destinationIndex = -1;

    private void Awake()
    {
        // Ensure the collider is set to be a trigger to prevent physics collisions.
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the object that entered is not the Player, or if our index is invalid, do nothing.
        if (!other.CompareTag("Player") || destinationIndex < 0)
        {
            return;
        }

        // If the NavigationManager exists, tell it to set the destination and turn on the path.
        if (NavigationManager.nevigationManager != null)
        {
            Debug.Log($"[NavigationTrigger] Player entered. Setting navigation path to index {destinationIndex}.");
            NavigationManager.nevigationManager.SetDestination(destinationIndex);
            NavigationManager.nevigationManager.EnableNavigation(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the Player leaves the trigger area, turn off the navigation path to keep the screen clean.
        if (other.CompareTag("Player"))
        {
            if (NavigationManager.nevigationManager != null)
            {
                Debug.Log("[NavigationTrigger] Player exited. Hiding navigation path.");
                NavigationManager.nevigationManager.EnableNavigation(false);
            }
        }
    }
}