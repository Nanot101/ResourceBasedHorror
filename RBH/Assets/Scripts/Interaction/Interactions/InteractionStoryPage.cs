using UnityEngine;

public class InteractionStoryPage : InteractionBase
{
    [SerializeField]
    private DialogueSystem dialogueSystem;

    [SerializeField]
    private StoryPage storyPage;

    private void Start()
    {
        Debug.Assert(dialogueSystem != null, $"{nameof(dialogueSystem)} is required for {gameObject.name}");
        Debug.Assert(storyPage != null, $"{nameof(storyPage)} is required for {gameObject.name}");
    }

    public override bool CanInteract(IInteractionCaller caller)
    {
        if (dialogueSystem.DialogueTriggered
            || caller.StoryPages.Contains(storyPage))
        {
            return false;
        }

        return base.CanInteract(caller);
    }

    public override void Interact(IInteractionCaller caller)
    {
        if (dialogueSystem.DialogueTriggered
            || caller.StoryPages.Contains(storyPage))
        {
            return;
        }

        caller.StoryPages.Add(storyPage);

        dialogueSystem.StartDialogue(storyPage.DialogueScript, storyPage.IconList);

        Destroy(gameObject);
    }
}
