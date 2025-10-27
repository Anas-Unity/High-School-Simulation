using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NavigationUI : MonoBehaviour
{
    [Header("UI References")]
    public Button navigationButton;
    public GameObject destinationListPanel;
    public Transform buttonContainer;
    public Button destinationButtonPrefab;

    [Header("Input Settings")]
    [Tooltip("Time window (in seconds) to detect a double-click.")]
    public float doubleClickTime = 0.3f;

    private float lastClickTime = 0f;
    private bool navigationActive = false;
    private List<DirectionTarget> availableTargets = new List<DirectionTarget>();
    private Image navButtonImage;

    private void Start()
    {
        if (destinationListPanel != null)
            destinationListPanel.SetActive(false);

        navButtonImage = navigationButton.GetComponent<Image>();

        navigationButton.onClick.AddListener(OnNavigationButtonClick);

        // Cache all direction targets in the scene
        DirectionTarget[] allTargets = FindObjectsOfType<DirectionTarget>();
        availableTargets.AddRange(allTargets);

        UpdateButtonVisual();
    }

    private void OnNavigationButtonClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;
        lastClickTime = Time.time;

        // Detect double-click → open destination list
        if (timeSinceLastClick <= doubleClickTime)
        {
            ToggleDestinationList();
            return;
        }

        // Single click → toggle navigation visibility
        ToggleNavigation();
    }

    private void ToggleNavigation()
    {
        navigationActive = !navigationActive;

        // Update visuals for active/inactive state
        UpdateButtonVisual();

        // Toggle the path drawing in NavigationManager
        //NavigationManager.Instance.OnNavigationButtonPressed();
    }

    private void ToggleDestinationList()
    {
        if (destinationListPanel == null) return;

        bool showList = !destinationListPanel.activeSelf;

        if (showList)
            PopulateDestinationList();

        destinationListPanel.SetActive(showList);
    }

    private void PopulateDestinationList()
    {
        // Clear existing buttons
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // Create new buttons for all DirectionTargets
        for (int i = 0; i < availableTargets.Count; i++)
        {
            int index = i;
            var target = availableTargets[i];

            Button btn = Instantiate(destinationButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<Text>().text = target.targetName;

            btn.onClick.AddListener(() =>
            {
                NavigationManager.Instance.SetDestination(index);
                destinationListPanel.SetActive(false);
            });
        }
    }

    private void UpdateButtonVisual()
    {
        if (navButtonImage == null) return;

        Color c = navButtonImage.color;
        c.a = navigationActive ? 1f : 0.4f; // full when active, dim when inactive
        navButtonImage.color = c;
    }
}
