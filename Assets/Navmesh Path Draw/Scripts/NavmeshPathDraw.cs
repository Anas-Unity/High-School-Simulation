/*using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]

public class NavmeshPathDraw : MonoBehaviour
{
    public Transform destination;
    public bool recalculatePath = true;
    public float recalculationTime = 0.1f;
    public LayerMask groundLayers;

    NavMeshPath path;
    LineRenderer lr;

    float time = 0f;
    bool stopped = false;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        path = new NavMeshPath();

        if (lr.materials.Length == 0)
        {
            lr.material = Resources.Load("material/path_mat", typeof(Material)) as Material;
        }

        Draw();
    }

    //draw the path
    public void Draw()
    {
        if (destination == null) return;
        stopped = false;

        RaycastHit downHit;
        Vector3 validatedDesPos;
        Vector3 validatedOriginPos;

        //GET THE NAVMESH POSITION BELOW DESTINATION AND ORIGIN IN ORDER TO PRINT THE PATH
        //validate destination position
        if (Physics.Raycast(destination.position, -Vector3.up, out downHit, Mathf.Infinity, groundLayers))
        {
            validatedDesPos = new Vector3(destination.position.x, downHit.transform.position.y, destination.position.z);
        }
        else
        {
            validatedDesPos = destination.position;
        }

        //validate origin position
        if (Physics.Raycast(transform.position, -Vector3.up, out downHit, Mathf.Infinity, groundLayers))
        {
            validatedOriginPos = new Vector3(transform.position.x, downHit.transform.position.y, transform.position.z);
        }
        else
        {
            validatedOriginPos = transform.position;
        }

        NavMesh.CalculatePath(validatedOriginPos, validatedDesPos, NavMesh.AllAreas, path);
        Vector3[] corners = path.corners;

        lr.positionCount = corners.Length;
        lr.SetPositions(corners);
    }

    //stop drawing the path
    public void Stop()
    {
        stopped = true;
        lr.positionCount = 0;
    }

    //recalculate the route ONCE every frame if enabled
    void Update()
    {
        if (!recalculatePath) return;
        if (!stopped) time += Time.deltaTime;

        if (time >= recalculationTime && !stopped)
        {
            time = 0f;
            Draw();
        }
    }
}
*/

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class NavmeshPathDraw : MonoBehaviour
{
    [Header("References")]
    public Transform player;         // ✅ Added
    public Transform destination;

    [Header("Settings")]
    public bool recalculatePath = true;
    public float recalculationTime = 0.1f;
    public LayerMask groundLayers;

    private NavMeshPath path;
    private LineRenderer lr;
    private float time = 0f;
    private bool stopped = false;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        path = new NavMeshPath();

        if (lr.material == null)
        {
            var mat = Resources.Load<Material>("material/path_mat");
            if (mat != null)
                lr.material = mat;
            else
                Debug.LogWarning("⚠️ No path material found in Resources/material/path_mat");
        }

        lr.startWidth = 0.15f;
        lr.endWidth = 0.15f;
        lr.enabled = false; // hidden until enabled
    }

    public void Draw()
    {
        if (destination == null || player == null)
        {
            Debug.LogWarning("⚠️ Missing player or destination in NavmeshPathDraw!");
            return;
        }

        stopped = false;
        lr.enabled = true;

        RaycastHit downHit;
        Vector3 validatedDesPos;
        Vector3 validatedOriginPos;

        // Validate destination position
        if (Physics.Raycast(destination.position, -Vector3.up, out downHit, Mathf.Infinity, groundLayers))
            validatedDesPos = downHit.point;
        else
            validatedDesPos = destination.position;

        // ✅ Validate player position (not our own transform)
        if (Physics.Raycast(player.position, -Vector3.up, out downHit, Mathf.Infinity, groundLayers))
            validatedOriginPos = downHit.point;
        else
            validatedOriginPos = player.position;

        if (NavMesh.CalculatePath(validatedOriginPos, validatedDesPos, NavMesh.AllAreas, path))
        {
            if (path.corners.Length > 1)
            {
                lr.positionCount = path.corners.Length;
                lr.SetPositions(path.corners);
                Debug.Log($"✅ Path drawn from {player.name} to {destination.name}, corners: {path.corners.Length}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Path found but only 1 corner from {player.name} to {destination.name}");
            }
        }
        else
        {
            Debug.LogWarning($"❌ Could not calculate NavMesh path from {player.name} to {destination.name}");
            lr.positionCount = 0;
        }
    }

    public void Stop()
    {
        stopped = true;
        lr.positionCount = 0;
        lr.enabled = false;
    }

    void Update()
    {
        if (!recalculatePath || stopped || !lr.enabled) return;

        time += Time.deltaTime;
        if (time >= recalculationTime)
        {
            time = 0f;
            Draw();
        }
    }
}
