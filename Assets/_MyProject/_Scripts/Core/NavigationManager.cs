using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    [Header("Navigation Settings")]
    public GameObject player;
    public GameObject navMeshPathDrawer;
    private LineRenderer lineRenderer;
    private NavMeshPath path;

    [Header("UI References")]
    public Button navigationButton; // Assign your button here
    public GameObject navigationListPanel; // Optional (for future destinations)

    [Header("Destinations")]
    public List<Transform> navigationTargets; // Assign School, Market, House etc.

    private bool navigationActive = false;
    private int currentDestinationIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        path = new NavMeshPath();

        if (navMeshPathDrawer != null)
            lineRenderer = navMeshPathDrawer.GetComponent<LineRenderer>();

        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (navigationListPanel != null)
            navigationListPanel.SetActive(false);
    }

    private void Start()
    {
        if (navigationButton != null)
        {
            navigationButton.onClick.RemoveAllListeners();
            navigationButton.onClick.AddListener(ToggleNavigation);
        }

        UpdateButtonVisual();
    }

    private void Update()
    {
        // Keep updating path dynamically as player moves
        if (navigationActive && currentDestinationIndex >= 0)
        {
            DrawPath(navigationTargets[currentDestinationIndex].position);
        }
    }

    // 🔹 Called by button or NPC to toggle navigation line
    public void ToggleNavigation()
    {
        navigationActive = !navigationActive;
        Debug.Log("Navigation Button Clicked!");


        if (lineRenderer != null)
            lineRenderer.enabled = navigationActive;

        if (!navigationActive)
        {
            lineRenderer.positionCount = 0; // clear line when turned off
        }

        UpdateButtonVisual();
    }

    // 🔹 Used by NPC to assign a target
    public void SetDestination(int index)
    {
        if (index < 0 || index >= navigationTargets.Count)
        {
            Debug.LogWarning("Invalid navigation target index!");
            return;
        }

        currentDestinationIndex = index;
        if (navigationActive)
            DrawPath(navigationTargets[index].position);
    }

    private void DrawPath(Vector3 destination)
    {
        if (player == null || lineRenderer == null)
            return;

        if (NavMesh.CalculatePath(player.transform.position, destination, NavMesh.AllAreas, path))
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
    }

    // 🔹 Shows or hides the navigation button (used by NPCDialogueTrigger)
    public void ShowNavigationButton(bool state)
    {
        if (navigationButton != null)
            navigationButton.gameObject.SetActive(state);
    }

    private void UpdateButtonVisual()
    {
        if (navigationButton == null) return;

        Color c = navigationButton.image.color;
        c.a = navigationActive ? 1f : 0.4f; // full alpha = active, faded = inactive
        navigationButton.image.color = c;
    }
}
