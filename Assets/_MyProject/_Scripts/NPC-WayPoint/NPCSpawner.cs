/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject NPCPrefab;
    public int NPCToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < NPCToSpawn)
        {
            Debug.Log("NPC "+count+" Spawned in the game");
            GameObject obj = Instantiate(NPCPrefab);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.position;

            yield return new WaitForSeconds(0.4f);
            count++;
        }
    }
}
*/

using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    // An array to hold multiple NPC prefabs.
    [Tooltip("A list of all the different NPC prefabs that can be spawned.")]
    public GameObject[] npcPrefabs;

    [Tooltip("The total number of NPCs to spawn in the scene.")]
    public int NPCToSpawn;

    [Tooltip("(Optional) The parent transform for all spawned NPCs. If left empty, a new object will be created automatically.")]
    public Transform npcParentContainer;

    void Start()
    {
        if (npcParentContainer == null)
        {
            npcParentContainer = new GameObject("--- Spawned NPCs ---").transform;
        }

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        // Safety check: Make sure there are prefabs to spawn.
        if (npcPrefabs == null || npcPrefabs.Length == 0)
        {
            Debug.LogError("NPC Spawner has no prefabs assigned in the 'Npc Prefabs' array.", this.gameObject);
            yield break; // Stop the coroutine
        }

        int count = 0;
        while (count < NPCToSpawn)
        {
            if (transform.childCount == 0)
            {
                Debug.LogError("NPC Spawner has no child waypoints to spawn NPCs at.", this.gameObject);
                yield break;
            }

            // 1. Pick a random prefab from the array.
            GameObject randomPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

            // 2. Instantiate the chosen random prefab.
            GameObject obj = Instantiate(randomPrefab);

            obj.transform.SetParent(npcParentContainer);

            Transform child = transform.GetChild(Random.Range(0, transform.childCount));
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.position;

            yield return new WaitForSeconds(0.5f);
            count++;
        }
    }
}