using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public List<string> questNames = new();
    public int coins = 500;
    public List<BondData> bonds;

    void Awake()
    {
        if (gameManager != null)
        {
            Destroy(gameObject);
        }
        gameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoins(int amount) => coins += amount;
    public void SpendCoins(int amount) => coins -= amount;
}
