using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

public class PlayerInteractionCaller : MonoBehaviour, IInteractionCaller
{
    [field: SerializeField] public StoryPageStore StoryPages { get; private set; }

    [field: SerializeField] public RecipePageStore RecipePages { get; private set; }

    [field: SerializeField] public ContainerHandler InventoryContainer { get; private set; }

    private readonly List<Interactable> interactables = new();

    private Interactable selectedInteractable = null;

    public GameObject GameObject => gameObject;

    private void Update()
    {
        if (GamePause.IsPaused)
        {
            SetInteractable(null);
            return;
        }

        SetInteractable(GetClosestPossibleInteractable());

        TryInteract();
    }

    private Interactable GetClosestPossibleInteractable()
        => interactables
            .Where(x => x.Interaction.CanInteract(this))
            .OrderBy(x => Vector2.Distance(gameObject.transform.position, x.Position))
            .FirstOrDefault();

    private void SetInteractable(Interactable interactable)
    {
        if (selectedInteractable == interactable)
        {
            return;
        }

        if (selectedInteractable != null)
        {
            selectedInteractable.HideUI();
        }

        selectedInteractable = interactable;

        if (selectedInteractable != null)
        {
            selectedInteractable.ShowUI();
        }
    }

    private void TryInteract()
    {
        if (selectedInteractable == null)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        selectedInteractable.Interaction.Interact(this);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.TryGetComponent<Interactable>(out var interactable))
        {
            //Debug.Log("Player interaction caller didn't enter interactable");
            return;
        }

        if (!interactables.Contains(interactable))
        {
            interactables.Add(interactable);

            //Debug.Log("interactable added");

            //return;
        }

        //Debug.Log("interactable already added");
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.TryGetComponent<Interactable>(out var interactable))
        {
            //Debug.Log("Player interaction caller didn't exit interactable");
            return;
        }

        if (interactables.Contains(interactable))
        {
            interactables.Remove(interactable);

            //Debug.Log("interactable removed");

            //return;
        }

        //Debug.Log("interactable wasn't in the list");
    }
}