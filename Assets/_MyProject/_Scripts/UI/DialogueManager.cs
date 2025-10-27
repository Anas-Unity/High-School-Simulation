using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Invector.vCharacterController;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public Action onDialogueEnd; // 👈 Add this event

    [Header("UI References")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button primaryButton;
    [SerializeField] private Button secondaryButton;

    private string[] lines;
    private int currentLine;
    private bool dialogueActive = false;
    private bool multiChoice = false;

    private vThirdPersonController controller;
    private vThirdPersonInput controllerInput;

    void Awake() => Instance = this;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            controller = player.GetComponent<vThirdPersonController>();
            controllerInput = player.GetComponent<vThirdPersonInput>();
        }

        if (primaryButton) primaryButton.onClick.AddListener(OnPrimaryButtonPressed);
        if (secondaryButton) secondaryButton.onClick.AddListener(OnSecondaryButtonPressed);

        dialogueUI.SetActive(false);
    }

    public void StartDialogue(DialogueData data)
    {
        if (dialogueActive) return;

        dialogueUI.SetActive(true);
        dialogueActive = true;

        nameText.text = data.characterName;
        lines = data.dialogueLines;
        currentLine = 0;
        dialogueText.text = lines[currentLine];

        primaryButton.gameObject.SetActive(true);
        secondaryButton.gameObject.SetActive(data.useTwoButtons);

        primaryButton.GetComponentInChildren<TMP_Text>().text = data.primaryButtonText;
        if (data.useTwoButtons)
            secondaryButton.GetComponentInChildren<TMP_Text>().text = data.secondaryButtonText;
    }

    private void OnPrimaryButtonPressed()
    {
        if (!dialogueActive) return;
        NextLine();
    }

    private void OnSecondaryButtonPressed()
    {
        if (!dialogueActive) return;
        CloseDialogue();
    }

    private void NextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
        }
        else
        {
            CloseDialogue();
        }
    }

    private void CloseDialogue()
    {
        dialogueUI.SetActive(false);
        dialogueActive = false;
        onDialogueEnd?.Invoke(); // 👈 Notify NPC trigger system
    }
}
