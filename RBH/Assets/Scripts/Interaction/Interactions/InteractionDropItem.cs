using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class InteractionDropItem : InteractionBase
{
    [SerializeField] private EnemyDropItem dropItem;

    private List<ItemStack> itemStacks;

    private void Start()
    {
        Debug.Assert(dropItem, $"{nameof(dropItem)} is required for {gameObject.name}", this);

        itemStacks = CreateInitialItemStacks();
    }

    public override bool CanInteract(IInteractionCaller caller) =>
        base.CanInteract(caller) && caller.InventoryContainer && caller.InventoryContainer.Container is not null;

    public override void Interact(IInteractionCaller caller)
    {
        if (!caller.InventoryContainer || caller.InventoryContainer.Container is null)
        {
            return;
        }

        if (TrySimpleAddItem(caller.InventoryContainer.Container))
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
        while (itemStacks.Count > 0)
        {
            var itemStackToAdd = itemStacks[^1];

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

                itemStacks.Remove(itemStackToAdd);

                return false;
            }

            if (remainingAmount > 0)
            {
                CreateNewStackWithRemainingAmount();
            }

            itemStacks.Remove(itemStackToAdd);

            continue;

            void CreateNewStackWithRemainingAmount()
            {
                var newItemStack = itemStackToAdd.Clone();
                newItemStack.SetAmount(remainingAmount);
                itemStacks.Add(newItemStack);
            }
        }

        return true;
    }

    private List<ItemStack> CreateInitialItemStacks()
    {
        var itemMaxStack = dropItem.DropSO.DropItemData.MaxStackSize;
        var itemAmount = Random.Range(dropItem.DropSO.MinQuantity, dropItem.DropSO.MaxQuantity + 1);

        var result = new List<ItemStack>();

        while (itemAmount > 0)
        {
            var newItemStackAmount = Mathf.Min(itemAmount, itemMaxStack);

            var newItemStack = new ItemStack(dropItem.DropSO.DropItemData, amount: newItemStackAmount);
            result.Add(newItemStack);

            itemAmount -= newItemStackAmount;
        }

        return result;
    }
}