using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NavigationButtonUI : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Index of this destination in NavigationManager.navigationTargets")]
    public int destinationIndex;

    [Tooltip("Optional manual override for button label. Leave empty to auto-use target name.")]
    public string customLabel;

    [SerializeField] private Button button;
    [SerializeField] private Text buttonText;
    [SerializeField] private NavigationManager navigationManager;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();

        // Try to find manager automatically
        navigationManager = NavigationManager.nevigationManager;
        if (navigationManager == null)
            Debug.LogWarning("⚠️ NavigationButtonUI: No NavigationManager found in scene!");
    }

    private void Start()
    {
        // Automatically set text
        UpdateButtonLabel();

        // Hook up button click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void UpdateButtonLabel()
    {
        if (buttonText == null) return;

        string label = customLabel;

        // Auto-pull name from navigation target if available
        if (string.IsNullOrEmpty(label) &&
            navigationManager != null &&
            destinationIndex >= 0 &&
            destinationIndex < navigationManager.navigationTargets.Count &&
            navigationManager.navigationTargets[destinationIndex] != null)
        {
            label = navigationManager.navigationTargets[destinationIndex].name;
        }

        if (string.IsNullOrEmpty(label))
            label = "Unknown";

        buttonText.text = label;
    }

    private void OnClick()
    {
        if (navigationManager == null)
        {
            Debug.LogWarning("⚠️ NavigationButtonUI: NavigationManager missing!");
            return;
        }

        navigationManager.SetDestination(destinationIndex);
        navigationManager.EnableNavigation(true);
    }
}
