using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectOnSignal : MonoBehaviour
{
    public GameObject player;
    public Vector3 newPlayerPosition;
    public void SetNewPosition()
    {
        player.transform.position = newPlayerPosition;
    }
}
