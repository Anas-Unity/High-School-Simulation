using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class QuestBookBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject questPage;
    [SerializeField] private Text questTextBox;
    [SerializeField] private GameObject bookNotification;
    [SerializeField] private string[] defaultQuestText;

    private bool openBook;

    public void OpenQuestBook() 
    {
        openBook = !openBook;
        CreatePage();
        WriteQuests();
    }

    private void CreatePage() 
    {
        if (questPage != null && bookNotification != null) 
        {
            if (openBook)
            {
                questPage.SetActive(true);
                bookNotification.SetActive(false);
            }
            else
            {
                questPage.SetActive(false);
            }
        }
    }
    private void WriteQuests() 
    {
        if (questTextBox != null) 
        {
            if(GameManager.gameManager.questNames.Count == 0) 
            {
                if (defaultQuestText != null) 
                {
                    int randomNumber = (Random.Range(0, defaultQuestText.Length));
                    questTextBox.text = defaultQuestText[randomNumber];
                        
                }
            }
            else 
            {
                StringBuilder stringBuilder = new();

                foreach (string quest in GameManager.gameManager.questNames) 
                {
                    stringBuilder.AppendLine(quest);
                }
                questTextBox.text = stringBuilder.ToString();
            }
            questTextBox.rectTransform.sizeDelta = new Vector2(questTextBox.rectTransform.sizeDelta.x, questTextBox.preferredHeight);
        }
    }

}
