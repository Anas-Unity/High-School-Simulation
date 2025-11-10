/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{

    NPCNavigationController controller;
    public Waypoint currentWaypoint;

    int direction;

    private void Awake()
    {
        controller = GetComponent<NPCNavigationController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0f, 1f));
        controller.SetDestination(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldBranch = false;

        if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
        {
            shouldBranch = Random.Range(0f,1f)<= currentWaypoint.branchRation ? true : false;
        }
        if (shouldBranch)
        {
            currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
        }
        else {
            if (controller.ReachedDestination)
            {
                if (direction == 0) // Move Forward
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else // At the end of the path, turn around
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                        direction = 1;
                    }
                }
                else if (direction == 1) // Move Backward
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else // At the start of the path, turn around
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        direction = 0;
                    }
                }
            }
            //Only set a new destination after choosing the next waypoint.
            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }
}
*/

using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    NPCNavigationController controller;
    public Waypoint currentWaypoint;
    int direction;

    private void Awake()
    {
        controller = GetComponent<NPCNavigationController>();
    }

    void Start()
    {
        direction = Mathf.RoundToInt(Random.Range(0f, 1f));
        // On start, get the position and the Y-axis flag from the first waypoint.
        SetNextDestination();
    }

    void Update()
    {
        // We only need to find a new waypoint if we've arrived at the old one.
        // This is much more efficient than checking every frame.
        if (controller.ReachedDestination)
        {
            bool shouldBranch = false;
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRation;
            }

            if (shouldBranch)
            {
                currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
            }
            else
            {
                if (direction == 0) // Move Forward
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                    else // At the end of the path, turn around
                    {
                        direction = 1;
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                }
                else if (direction == 1) // Move Backward
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else // At the start of the path, turn around
                    {
                        direction = 0;
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                }
            }
            // Once the next waypoint is chosen, set the destination.
            SetNextDestination();
        }
    }

    /// <summary>
    /// Gets the position and Y-update flag from the current waypoint and sends it to the controller.
    /// </summary>
    void SetNextDestination()
    {
        if (currentWaypoint != null)
        {
            Vector3 targetPosition = currentWaypoint.GetPosition();
            bool shouldUpdateY = currentWaypoint.updateYPosition;
            controller.SetDestination(targetPosition, shouldUpdateY);
        }
    }
}