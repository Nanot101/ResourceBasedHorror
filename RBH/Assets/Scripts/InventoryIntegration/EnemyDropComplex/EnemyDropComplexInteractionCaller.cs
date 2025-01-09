using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using UnityEngine;

public class EnemyDropComplexInteractionCaller : MonoBehaviour, IInteractionCaller
{
    [SerializeField] private float itemPickUpRadius = 2.0f;

    public GameObject GameObject => null;
    public StoryPageStore StoryPages => null;
    public RecipePageStore RecipePages => null;
    public Container InventoryContainer { get; private set; }

    public void PickUpItemsAroundPlayer(Vector3 playerPosition, Container inventoryContainer)
    {
        InventoryContainer = inventoryContainer;

        var overlappedColliders = Physics2D.OverlapCircleAll(playerPosition, itemPickUpRadius);

        foreach (var interactable in GetDropItemInteractablesFromColliders(overlappedColliders)
                     .OrderBy(x => Vector2.Distance(gameObject.transform.position, x.Position)))
        {
            interactable.Interaction.Interact(this);
        }
    }

    private IEnumerable<Interactable> GetDropItemInteractablesFromColliders(IEnumerable<Collider2D> colliders)
    {
        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<Interactable>(out var interactable))
            {
                continue;
            }

            if (interactable.Interaction is not InteractionDropItem)
            {
                continue;
            }

            if (!interactable.Interaction.CanInteract(this))
            {
                continue;
            }

            yield return interactable;
        }
    }
}