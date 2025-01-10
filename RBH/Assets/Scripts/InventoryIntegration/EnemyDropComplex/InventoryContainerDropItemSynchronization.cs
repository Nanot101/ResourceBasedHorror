using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryContainerDropItemSynchronization : MonoBehaviour
{
    [SerializeField] private InventoryDropSystem dropSystem;

    [SerializeField] private float itemPickUpRadius = 2.0f;

    private readonly Dictionary<ItemData, EnemyDropItem> enemyDropItemsInRadius = new();
    private Collider2D[] overlappedColliders = new Collider2D[10];
    private Container inventoryContainer;
    private Vector3 playerPosition;

    private void Start()
    {
        Debug.Assert(dropSystem,
            $"{nameof(dropSystem)} is required for {nameof(InventoryContainerDropItemSynchronization)}", this);
    }

    public void StartSynchronization(Vector3 playerPosition, Container inventoryContainer)
    {
        this.inventoryContainer = inventoryContainer;
        this.playerPosition = playerPosition;

        SetOverlappedCollidersInRadius();
        SetEnemyDropItemsInRadius();
        AddItemsInRadiusToContainer();
        SubscribeToContainerEvents();
    }

    public void StopSynchronization()
    {
        inventoryContainer.OnItemPlacedAt -= OnItemPlacedAt;
        inventoryContainer.OnItemRootRemoved -= OnItemRootRemoved;
    }

    private void SetOverlappedCollidersInRadius()
    {
        while (true)
        {
            var overlappedCollidersCount =
                Physics2D.OverlapCircleNonAlloc(playerPosition, itemPickUpRadius, overlappedColliders);

            if (overlappedCollidersCount < overlappedColliders.Length)
            {
                break;
            }

            overlappedColliders = new Collider2D[overlappedColliders.Length * 2];
        }
    }

    private void SetEnemyDropItemsInRadius()
    {
        enemyDropItemsInRadius.Clear();

        var enemyDropItemsInRadiusQueryGroupedAndOrdered = SelectEnemyDropItemFromCollider2D(overlappedColliders)
            .DistinctBy(x => x.gameObject) // sometimes enemy drop item is duplicated
            .OrderBy(x => Vector2.Distance(playerPosition, x.transform.position))
            .GroupBy(x => x.ItemData);

        foreach (var itemGroup in enemyDropItemsInRadiusQueryGroupedAndOrdered)
        {
            foreach (var dropItem in itemGroup)
            {
                if (enemyDropItemsInRadius.TryAdd(itemGroup.Key, dropItem))
                {
                    continue;
                }

                MergeDropItemStacks(enemyDropItemsInRadius[itemGroup.Key].ItemStacks, dropItem.ItemStacks);
                Destroy(dropItem.gameObject);
            }
        }
    }

    private static IEnumerable<EnemyDropItem> SelectEnemyDropItemFromCollider2D(IEnumerable<Collider2D> colliders)
    {
        foreach (var collider in colliders)
        {
            if (collider && collider.TryGetComponent<EnemyDropItem>(out var enemyDropItem))
            {
                yield return enemyDropItem;
            }
        }
    }

    private static void MergeDropItemStacks(List<ItemStack> masterItemStacks, List<ItemStack> slaveItemStacks)
    {
        // after sorting last stack may have its amount less than max stack size
        masterItemStacks.Sort((a, b) => a.Amount.CompareTo(b.Amount));

        foreach (var slaveItemStack in slaveItemStacks)
        {
            var lastMasterItemStack = masterItemStacks[^1];
            var freeMasterStackSpace = lastMasterItemStack.ItemData.MaxStackSize - lastMasterItemStack.Amount;
            var possibleAmountToAddToMaster = math.min(freeMasterStackSpace, slaveItemStack.Amount);

            if (possibleAmountToAddToMaster == 0)
            {
                // last master item stack if full, so add slave item stack
                masterItemStacks.Add(slaveItemStack);
                continue;
            }

            // last master item stack has some space, so merge slave stack to master stack

            lastMasterItemStack.SetAmount(lastMasterItemStack.Amount + possibleAmountToAddToMaster);

            var amountLeftInSlave = slaveItemStack.Amount - possibleAmountToAddToMaster;

            if (amountLeftInSlave <= 0)
            {
                continue;
            }

            // slave item stack still has items left so set new amount to left amount and add slave stack

            slaveItemStack.SetAmount(amountLeftInSlave);
            masterItemStacks.Add(slaveItemStack);
        }
    }

    private void AddItemsInRadiusToContainer()
    {
        foreach (var itemStack in enemyDropItemsInRadius.Values.SelectMany(x => x.ItemStacks))
        {
            _ = inventoryContainer.AddItem(itemStack, out _);
        }
    }

    private void SubscribeToContainerEvents()
    {
        inventoryContainer.OnItemPlacedAt += OnItemPlacedAt;
        inventoryContainer.OnItemRootRemoved += OnItemRootRemoved;
    }

    private void OnItemPlacedAt(object sender, ItemPlacedAtEventArgs args)
    {
        var placedStack = args.ItemStack;

        if (enemyDropItemsInRadius.TryGetValue(args.ItemStack.ItemData, out var dropItemWithQualifiedItemStack))
        {
            AddStackToExistingDropItem(dropItemWithQualifiedItemStack, placedStack);

            return;
        }

        // item isn't on the ground so add new enemy drop item

        var newItemStackList = new List<ItemStack>
        {
            new(placedStack.ItemData, amount: placedStack.Amount)
        };

        var newDropItem = dropSystem.DropItem(placedStack.ItemData, newItemStackList, playerPosition);
        enemyDropItemsInRadius.Add(newDropItem.ItemData, newDropItem);
    }

    private static void AddStackToExistingDropItem(EnemyDropItem dropItemWithQualifiedItemStack, ItemStack placedStack)
    {
        var lastStack = dropItemWithQualifiedItemStack.ItemStacks[^1];
        var freeStackSpace = lastStack.ItemData.MaxStackSize - lastStack.Amount;
        var possibleAmountToAddToLastStack = math.min(freeStackSpace, placedStack.Amount);

        if (possibleAmountToAddToLastStack == 0)
        {
            // last item stack if full, so add new item stack
            dropItemWithQualifiedItemStack.ItemStacks.Add(new ItemStack(placedStack.ItemData,
                amount: placedStack.Amount));

            return;
        }

        // last item stack has some space, so fill last stack

        lastStack.SetAmount(lastStack.Amount + possibleAmountToAddToLastStack);

        var amountLeftInRemovedStack = placedStack.Amount - possibleAmountToAddToLastStack;

        if (amountLeftInRemovedStack <= 0)
        {
            return;
        }

        // not all items from the placed stack fit into the last stack, so add new stack with left amount

        var newStack = new ItemStack(placedStack.ItemData, amount: amountLeftInRemovedStack);
        dropItemWithQualifiedItemStack.ItemStacks.Add(newStack);
    }

    private void OnItemRootRemoved(object sender, ItemRootRemovedEventArgs args)
    {
        if (!enemyDropItemsInRadius.TryGetValue(args.ItemStack.ItemData, out var dropItemWithQualifiedItemData))
        {
            Debug.LogError("The removed item stack was not in the enemy drop items. " +
                           "Possible synchronization error between container and items on the ground!", this);
            return;
        }

        var itemStackToRemove = dropItemWithQualifiedItemData.ItemStacks.Find(y =>
            y.ItemData == args.ItemStack.ItemData && y.Amount == args.ItemStack.Amount);

        dropItemWithQualifiedItemData.ItemStacks.Remove(itemStackToRemove);

        if (dropItemWithQualifiedItemData.ItemStacks.Count > 0)
        {
            return;
        }

        Destroy(dropItemWithQualifiedItemData.gameObject);
        enemyDropItemsInRadius.Remove(args.ItemStack.ItemData);
    }
}