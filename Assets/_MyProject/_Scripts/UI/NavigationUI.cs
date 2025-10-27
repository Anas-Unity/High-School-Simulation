using UnityEngine;
using UnityEngine.UI;

public class NavigationUI : MonoBehaviour
{
    public Button navigationButton;

    private void Start()
    {
        if (navigationButton != null)
            navigationButton.gameObject.SetActive(false); // Hidden by default
    }

    public void ShowButton()
    {
        if (navigationButton != null)
            navigationButton.gameObject.SetActive(true);
    }
}
