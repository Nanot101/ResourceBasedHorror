using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogueSystem : MonoBehaviour
{
    public float dialogueSpeed;

    public List<Texture> icons = new List<Texture>();

    public TMP_Text nameText;
    public TMP_Text lineText;
    public RawImage characterIcon;

    public GameObject dialogueScreen;
    //Player Interaction needs to be added here

    //public GameObject player;

    //public AudioSource clickSound;

    private string dialogueScript;
    private string dialogueLine;

    private string nameStr;
    private string line;

    private int currentPos = 0;

    public bool DialogueTriggered { get; private set; } = false;

    private bool endOfDialogue = false;

    private Action onDialogEndedCallback;

    void Start()
    {
        currentPos = 0;
        DialogueTriggered = false;
        endOfDialogue = false;
        dialogueScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown("e") && DialogueTriggered)
        {
            //clickSound.Play();
            if (endOfDialogue)
                EndDialogue();
            else
                NextLine();
        }
    }

    private void OnDestroy()
    {
        onDialogEndedCallback = null;
    }

    public void StartDialogue(TextAsset dialogueFile, List<Texture> iconList, Action onDialogEndedCallback)
    {
        this.onDialogEndedCallback = onDialogEndedCallback;

        StartDialogue(dialogueFile, iconList);
    }

    public void StartDialogue(TextAsset dialogueFile, List<Texture> iconList)
    {
        GamePause.RequestPause<DialogueSystem>();
        Time.timeScale = 0;
        
        currentPos = 0;

        dialogueScript = dialogueFile.text;
        icons = iconList;

        dialogueScreen.SetActive(true);

        NextLine();

        DialogueTriggered = true;
    }

    void NextLine()
    {
        if (dialogueScript.IndexOf("\n", currentPos) < 0)
        {
            dialogueLine = dialogueScript.Substring(currentPos, dialogueScript.Length - currentPos);

            nameStr = dialogueLine.Substring(0, dialogueLine.IndexOf(":"));
            line = dialogueLine.Substring(dialogueLine.IndexOf(":") + 1, dialogueLine.Length - nameStr.Length - 1);

            StartCoroutine(ReadLine());

            endOfDialogue = true;
            return;
        }

        dialogueLine = dialogueScript.Substring(currentPos, dialogueScript.IndexOf("\n", currentPos) - currentPos);
        nameStr = dialogueLine.Substring(0, dialogueLine.IndexOf(":"));
        line = dialogueLine.Substring(dialogueLine.IndexOf(":") + 1, dialogueLine.Length - nameStr.Length - 1);

        currentPos = dialogueScript.IndexOf("\n", currentPos) + 1;

        StartCoroutine(ReadLine());
    }

    IEnumerator ReadLine()
    {
        //Display all dialogue with name and line displayed and with their corresponding photo
        nameText.text = nameStr;
        ChangeIcon(nameStr);
        lineText.text = "";
        while (lineText.text.Length < line.Length)
        {
            lineText.text += line.Substring(lineText.text.Length, 1);
            yield return new WaitForSecondsRealtime(dialogueSpeed);
        }
    }

    void ChangeIcon(string nameOfIcon)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            Debug.Log(icons[i].name);
            if (nameOfIcon == icons[i].name)
            {
                characterIcon.texture = icons[i];
                return;
            }
        }
    }

    void EndDialogue()
    {
        currentPos = 0;

        endOfDialogue = false;
        DialogueTriggered = false;
        dialogueScreen.SetActive(false);

        InvokeCallback();

        GamePause.RequestResume<DialogueSystem>();
        Time.timeScale = 1;

        //End dialogue
    }

    private void InvokeCallback()
    {
        if (onDialogEndedCallback == null)
        {
            return;
        }

        onDialogEndedCallback();

        onDialogEndedCallback = null;
    }
}