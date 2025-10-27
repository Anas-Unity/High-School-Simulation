using UnityEngine;

public class DrawAndStopInputs : MonoBehaviour
{
    public NavmeshPathDraw navmeshDraw;

    // Update is called once per frame
    void Update()
    {
        if(ControlFreak2.CF2Input.GetKeyDown(KeyCode.S)){
            navmeshDraw.Stop();
        }

        if(ControlFreak2.CF2Input.GetKeyDown(KeyCode.A)){
            navmeshDraw.Draw();
        }
    }
}
