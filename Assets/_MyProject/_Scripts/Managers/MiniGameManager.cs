using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    public void StartMiniGame(GameObject miniGamePrefab)
    {
        Instantiate(miniGamePrefab);
    }

    public void EndMiniGame(int score)
    {
        // add reward, increase bond, etc.
    }
}
