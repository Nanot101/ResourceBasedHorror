using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractionCaller : MonoBehaviour, IInteractionCaller
{
    private List<Interactable> interactables = new();

    private Interactable selectedInteracteable = null;

    public GameObject GameObject => gameObject;

    private void Update()
    {
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
        if (selectedInteracteable == interactable)
        {
            return;
        }

        if (selectedInteracteable != null)
        {
            selectedInteracteable.HideUI();
        }

        selectedInteracteable = interactable;

        if (selectedInteracteable != null)
        {
            selectedInteracteable.ShowUI();
        }
    }

    private void TryInteract()
    {
        if (selectedInteracteable == null)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        selectedInteracteable.Interaction.Interact(this);
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
