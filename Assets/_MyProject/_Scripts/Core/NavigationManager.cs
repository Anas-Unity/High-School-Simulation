using UnityEngine;
using System.Collections.Generic;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager nevigationManager;

    [Header("References")]
    public GameObject player;
    public NavmeshPathDraw pathDrawer;
    public List<Transform> navigationTargets; // House, School, Shop, etc.

    private bool navigationActive = false;
    private int currentDestinationIndex = -1;

    private void Awake()
    {
        if (nevigationManager == null) nevigationManager = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (pathDrawer != null && player != null)
            pathDrawer.player = player.transform;
    }

    public void SetDestination(int index)
    {
        if (index < 0 || index >= navigationTargets.Count)
        {
            Debug.LogWarning("⚠️ Invalid destination index for NavigationManager.");
            return;
        }

        currentDestinationIndex = index;
        Transform target = navigationTargets[index];

        if (pathDrawer == null)
        {
            Debug.LogWarning("⚠️ Missing PathDrawer reference!");
            return;
        }

        pathDrawer.destination = target;

        if (navigationActive)
            pathDrawer.Draw();

        Debug.Log($"🧭 Destination set to: {target.name}");
    }

    public void EnableNavigation(bool enable)
    {
        if (pathDrawer == null)
        {
            Debug.LogWarning("⚠️ Missing PathDrawer reference in NavigationManager.");
            return;
        }

        navigationActive = enable;

        if (enable)
        {
            pathDrawer.recalculatePath = true;
            pathDrawer.Draw();
        }
        else
        {
            pathDrawer.Stop();
        }
    }

    public void ToggleNavigation()
    {
        EnableNavigation(!navigationActive);
    }
}
