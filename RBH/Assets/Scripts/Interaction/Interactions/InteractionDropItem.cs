using InventorySystem;
using UnityEngine;

public class InteractionDropItem : InteractionBase
{
    [SerializeField] private EnemyDropItem dropItem;

    private void Start()
    {
        Debug.Assert(dropItem, $"{nameof(dropItem)} is required for {gameObject.name}", this);
    }

    public override bool CanInteract(IInteractionCaller caller) =>
        base.CanInteract(caller) && caller.InventoryContainer is not null;

    public override void Interact(IInteractionCaller caller)
    {
        if (caller.InventoryContainer is null)
        {
            return;
        }

        if (TrySimpleAddItem(caller.InventoryContainer))
        {
            Destroy(gameObject);
            return;
        }

        if (!EnemyDropComplexInventoryController.TryGetInstance(out var complexController))
        {
            Debug.LogError("Unable to find complex controller. Aborting interaction.");
            return;
        }

        complexController.StartDropComplexInventory(caller.GameObject.transform);
    }

    private bool TrySimpleAddItem(Container inventoryContainer)
    {
        while (dropItem.ItemStacks.Count > 0)
        {
            var itemStackToAdd = dropItem.ItemStacks[^1];

            if (!inventoryContainer.AddItem(itemStackToAdd, out var remainingAmount))
            {
                // Adding stack failed
                if (remainingAmount > 0 && remainingAmount == itemStackToAdd.Amount)
                {
                    // Adding full stack failed
                    return false;
                }

                // Part of the stack has been successfully added.
                // So we need to create a new stack with the remaining number of items.
                CreateNewStackWithRemainingAmount();

                dropItem.ItemStacks.Remove(itemStackToAdd);

                return false;
            }

            if (remainingAmount > 0)
            {
                CreateNewStackWithRemainingAmount();
            }

            dropItem.ItemStacks.Remove(itemStackToAdd);

            continue;

            void CreateNewStackWithRemainingAmount()
            {
                var newItemStack = itemStackToAdd.Clone();
                newItemStack.SetAmount(remainingAmount);
                dropItem.ItemStacks.Add(newItemStack);
            }
        }

        return true;
    }
}