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
    public Button navigationButton;
    public GameObject navigationListPanel;

    [Header("Destinations")]
    public List<Transform> navigationTargets; // Assign School, Market, House etc in Inspector

    private bool navigationActive;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        path = new NavMeshPath();
        lineRenderer = navMeshPathDrawer.GetComponent<LineRenderer>();
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        navigationListPanel.SetActive(false);
    }

    private void Start()
    {
        navigationButton.onClick.AddListener(OnNavigationButtonPressed);
        UpdateButtonVisual();
    }

    private float lastClickTime;
    private float doubleClickDelay = 0.3f;

    private void OnNavigationButtonPressed()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        if (timeSinceLastClick <= doubleClickDelay)
        {
            // Double-click -> show/hide list
            navigationListPanel.SetActive(!navigationListPanel.activeSelf);
        }
        else
        {
            // Single click -> toggle navigation
            navigationActive = !navigationActive;
            lineRenderer.enabled = navigationActive;
            UpdateButtonVisual();
        }
    }

    private void UpdateButtonVisual()
    {
        Color color = navigationButton.image.color;
        color.a = navigationActive ? 1f : 0.4f; // 40% transparency when inactive
        navigationButton.image.color = color;
    }

    public void SetDestination(int index)
    {
        if (index < 0 || index >= navigationTargets.Count) return;
        Transform target = navigationTargets[index];
        DrawPath(target.position);
        navigationListPanel.SetActive(false);
    }

    private void DrawPath(Vector3 destination)
    {
        if (!navigationActive) return;

        if (NavMesh.CalculatePath(player.transform.position, destination, NavMesh.AllAreas, path))
        {
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
    }
}
