using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private string questData;
    [SerializeField] private GameObject Notification;
    private bool questAdded = false;

    public void CreateQuest()
    {
        if (questData != null && !questAdded)
        {
            questAdded = !questAdded;
            GameManager.gameManager.questNames.Add(questData);
        }
        if(Notification != null && !questAdded) 
        {
            Notification.SetActive(true);
        }
    }
    public void CompleteQuest() 
    {
        if (questData != null && GameManager.gameManager.questNames.Contains(questData))
        {
            GameManager.gameManager.questNames.Remove(questData);
        }
    }
}
