/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint nextWaypoint;
    public Waypoint previousWaypoint;

    [Range(0f, 5f)]
    public float width = 2f;

    public List<Waypoint> branches = new List<Waypoint>();

    [Range(0f, 1f)]
    public float branchRation = 0.5f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position - transform.right * width/2f;
        Vector3 maxBound = transform.position + transform.right * width/2f;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }
}
*/

using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint nextWaypoint;
    public Waypoint previousWaypoint;

    [Range(0f, 5f)]
    public float width = 2f;

    // This boolean will appear in the Inspector for each waypoint.
    [Tooltip("Check this if the NPC should move to this waypoint's exact Y-level (for stairs, ramps, etc.).")]
    public bool updateYPosition = false;

    public List<Waypoint> branches = new List<Waypoint>();

    [Range(0f, 1f)]
    public float branchRation = 0.5f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position - transform.right * width / 2f;
        Vector3 maxBound = transform.position + transform.right * width / 2f;

        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }
}