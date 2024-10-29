using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDialog : InteractionBase
{
    [SerializeField]
    private DialogueSystem dialogueSystem;

    [SerializeField]
    private TextAsset dialogueScript;

    [SerializeField]
    private List<Texture> iconList = new();

    [SerializeField]
    private float dialogueRestartDelay = 1.0f;

    private bool dialogStarted;

    private void Start()
    {
        Debug.Assert(dialogueSystem != null, $"{nameof(dialogueSystem)} is required for {gameObject.name}");
        Debug.Assert(dialogueScript != null, $"{nameof(dialogueScript)} is required for {gameObject.name}");
        Debug.Assert(dialogueRestartDelay > 0.0f, $"{nameof(dialogueRestartDelay)} must be greater than 0");
    }

    public override bool CanInteract(IInteractionCaller caller)
        => base.CanInteract(caller) && !dialogStarted && !dialogueSystem.DialogueTriggered;

    public override void Interact(IInteractionCaller caller)
    {
        if (dialogStarted || dialogueSystem.DialogueTriggered)
        {
            return;
        }

        dialogStarted = true;

        dialogueSystem.StartDialogue(dialogueScript, iconList, StartDialogueDelayRestart);
    }

    private void StartDialogueDelayRestart()
    {
        StopAllCoroutines();
        StartCoroutine(DialogueRestartDelay());
    }

    private IEnumerator DialogueRestartDelay()
    {
        yield return new WaitForSeconds(dialogueRestartDelay);

        dialogStarted = false;
    }
}
