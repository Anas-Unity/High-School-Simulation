using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NavigationUI : MonoBehaviour
{
    [Header("References")]
    public NavigationManager navigationManager;
    public Transform destinationButtonContainer;
    public List<Button> destinationButtons = new List<Button>();

    [Header("UI Behavior")]
    public GameObject destinationListPanel;
    public Button togglePanelButton;
    public bool hideListAfterSelection = true;

    private void Start()
    {
        if (navigationManager == null)
            navigationManager = NavigationManager.nevigationManager;

        if (destinationListPanel != null)
            destinationListPanel.SetActive(false);

        SetupDestinationButtons();

        if (togglePanelButton != null)
        {
            togglePanelButton.onClick.RemoveAllListeners();
            togglePanelButton.onClick.AddListener(ToggleDestinationList);
        }
    }

    private void SetupDestinationButtons()
    {
        if (navigationManager == null || navigationManager.navigationTargets.Count == 0)
        {
            Debug.LogWarning("⚠️ NavigationManager missing or has no targets.");
            return;
        }

        // Ensure button count matches target count
        for (int i = 0; i < destinationButtons.Count; i++)
        {
            int index = i;

            if (destinationButtons[i] == null)
                continue;

            string targetName = "Unknown";

            if (index < navigationManager.navigationTargets.Count &&
                navigationManager.navigationTargets[index] != null)
            {
                targetName = navigationManager.navigationTargets[index].name;
            }

            // ✅ Update button label properly
            Text label = destinationButtons[i].GetComponentInChildren<Text>(true);
            if (label != null)
                label.text = targetName;

            // ✅ Hook up button click
            destinationButtons[i].onClick.RemoveAllListeners();
            destinationButtons[i].onClick.AddListener(() =>
            {
                navigationManager.SetDestination(index);
                navigationManager.EnableNavigation(true);

                if (hideListAfterSelection && destinationListPanel != null)
                    destinationListPanel.SetActive(false);
            });
        }
    }

    public void ToggleDestinationList()
    {
        if (destinationListPanel == null) return;
        destinationListPanel.SetActive(!destinationListPanel.activeSelf);
    }

}
