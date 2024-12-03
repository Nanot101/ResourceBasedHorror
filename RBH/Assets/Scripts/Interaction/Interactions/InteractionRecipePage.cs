using UnityEngine;

public class InteractionRecipePage : InteractionBase
{
    [SerializeField]
    private RecipePage recipePage;

    private void Start()
    {
        Debug.Assert(recipePage != null, $"{nameof(recipePage)} is required for {gameObject.name}");
    }

    public override bool CanInteract(IInteractionCaller caller)
    {
        if (caller.RecipePages.Contains(recipePage))
        {
            return false;
        }

        return base.CanInteract(caller);
    }

    public override void Interact(IInteractionCaller caller)
    {
        if (caller.RecipePages.Contains(recipePage))
        {
            return;
        }

        caller.RecipePages.Add(recipePage);

        Destroy(gameObject);
    }
}
