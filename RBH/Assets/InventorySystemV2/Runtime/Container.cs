using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{

    public class Container
    {
        private int containerSlots;

        public int containerWidth;
        public readonly List<ItemSlot> itemSlots = new List<ItemSlot>();
        public bool IsFull => itemSlots.Count >= containerSlots;
        public bool HasSpace => itemSlots.Count >= containerSlots;
        public bool ItemAmount => itemSlots.Count >= containerSlots;
        public int SlotCount => containerSlots;
        public int RemainingSpace => containerSlots - itemSlots.Count;

        public event EventHandler<ItemRootRemovedEventArgs> OnItemRootRemoved;

        public event EventHandler<ItemPlacedAtEventArgs> OnItemPlacedAt;

        public Container(string _containerName, int _containerSlots, int _containerWidth, IEnumerable<ItemStack> _initialItems = null)
        {
            containerWidth = _containerWidth;
            containerSlots = _containerSlots;
            int initialItemCount = _initialItems != null ? _initialItems.Count() : 0;
            for (int i = 0; i < containerSlots; i++)
            {
                itemSlots.Add(new ItemSlot(this, i, GetGridPos(i)));
            }
            //Debug.Log($"Initial item count: {initialItemCount}");
            for (int i = 0; i < initialItemCount; i++)
            {
                //Debug.Log($"Adding item: {_initialItems.ElementAt(i).ItemData.ItemName}");
                AddItem(_initialItems.ElementAt(i), out int _);
            }
        }

        public bool HasItem(ItemData data)
        {
            return itemSlots.Any(slot => slot.ItemData == data);
        }

        public int GetItemCount(ItemData data)
        {
            int count = 0;
            foreach (ItemSlot slot in itemSlots)
            {
                if (slot.ItemData == data)
                {
                    count += slot.Amount;
                }
            }
            return count;
        }

        public bool AddItem(ItemStack itemStack, out int remainingAmount)
        {
            //Search for a slot that can accept the item || Find first itemslot that has this item and can accept it
            //If it finds a slot and the item stack is maxxed out try to add the remaining amount to another slot
            //If there is no slot that can accept the item, try to add the item to an empty slot
            //If there is no slot empty and all maxed out return remaining amount of the tried added item
            //Debug.Log($"Attempting to add item: {itemStack.ItemData.ItemName}, Amount: {itemStack.Amount}");
            remainingAmount = itemStack.Amount;
            Debug.Log(remainingAmount +"remaining");
            if (itemStack.ItemData.IsStackable)
            {
                //Check if there already is a stack of the item
                foreach (var slot in itemSlots)
                {
                    if (slot.HasItemStack && slot.GetItemStack().ItemData == itemStack.ItemData && !slot.IsFull)
                    {
                        int spaceLeft = itemStack.ItemData.MaxStackSize - itemStack.Amount;
                        int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);
                        slot.AddAmount(amountToAdd, out remainingAmount);

                        Debug.Log($"Added {amountToAdd} to existing stack in slot {slot.Index}. Remaining amount: {remainingAmount} with max {slot.GetItemStack().ItemData.MaxStackSize}");

                        if (remainingAmount <= 0)
                        {
                            Debug.Log($"Successfully added item: {itemStack.ItemData.ItemName}");
                            return true;
                        }
                    }
                }

                //If the item is stackable and has size 1
                if (itemStack.ItemData.Size == Vector2Int.one)
                {
                    //Debug.Log("Item is stackable and size is 1x1. Trying to add to empty slots.");
                    for (int index = 0; index < itemSlots.Count; index++)
                    {
                        if (itemSlots[index].IsEmpty)
                        {
                            int amountToAdd = Mathf.Min(remainingAmount, itemStack.ItemData.MaxStackSize);
                            ItemStack newStack = new ItemStack(itemStack.ItemData, itemStack.IsRotated, amountToAdd);
                            itemSlots[index].SetItemStack(newStack, Vector2Int.zero, Vector2Int.one, index);
                            remainingAmount -= amountToAdd;

                            Debug.Log($"Created new stack in slot {index}. Amount added: {amountToAdd}. Remaining amount: {remainingAmount}");

                            if (remainingAmount <= 0)
                            {
                                Debug.Log("Successfully added all items to empty slots.");
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    Vector2Int itemSize = itemStack.IsRotated
                        ? new(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x)
                        : itemStack.ItemData.Size;

                    // Keep placing as many stacks as possible until no more remain
                    while (remainingAmount > 0)
                    {
                        // Find a free region of size itemSize
                        bool placedStack = false;
                        for (int index = 0; index < itemSlots.Count; index++)
                        {
                            Vector2Int gridPos = GetGridPos(index);
                            if (CanPlaceItemAt(gridPos.x, gridPos.y, itemSize))
                            {
                                int amountToAdd = Mathf.Min(remainingAmount, itemStack.ItemData.MaxStackSize);

                                ItemStack newStack = new ItemStack(
                                    itemStack.ItemData,
                                    itemStack.IsRotated,
                                    amountToAdd
                                );

                                PlaceItemAt(newStack, gridPos.x, gridPos.y, itemSize, out int _);

                                remainingAmount -= amountToAdd;
                                placedStack = true;
                                break;
                            }
                        }

                        if (!placedStack) { break; }
                    }

                    return (remainingAmount <= 0);
                }
            }
            //If the item is not stackable or has size greater than 1
            else
            {
                Debug.Log("Item is non-stackable or larger than 1x1. Trying to place in grid.");

                if (!itemStack.ItemData.IsStackable && itemStack.Amount > 1)
                {
                    Debug.LogWarning("Trying to add a non stackable item with amount greater than 1");
                    remainingAmount = itemStack.Amount - 1;
                    itemStack.SetAmount(1);
                }
                Vector2Int itemSize;
                if (!itemStack.IsRotated)
                {
                    itemSize = itemStack.ItemData.Size;
                }
                else
                {
                    itemSize = new(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x);
                }

                for (int index = 0; index < itemSlots.Count; index++)
                {
                    Vector2Int gridPos = GetGridPos(index);

                    Debug.Log($"Checking if item can be placed at position {gridPos}.");

                    if (CanPlaceItemAt(gridPos.x, gridPos.y, itemSize))
                    {
                        PlaceItemAt(itemStack, gridPos.x, gridPos.y, itemSize, out int _);

                        remainingAmount -= itemStack.Amount;

                        Debug.Log($"Item placed at position {gridPos}. Remaining amount: {remainingAmount}");

                        return true;
                    }
                    else
                    {
                        Debug.Log($"Cannot place item at position {gridPos}.");
                    }
                }
            }
            Debug.LogError($"Couldn't add item: {itemStack.ItemData.ItemName}");
            return false;
        }

        public bool AddItemAt(ItemStack itemStack, int startX, int startY)
        {
            if (!itemStack.IsRotated)
            {
                if (CanPlaceItemAt(startX, startY, itemStack.ItemData.Size))
                {
                    PlaceItemAt(itemStack, startX, startY, itemStack.ItemData.Size, out _);
                    return true;
                }
            }
            else
            {
                if (CanPlaceItemAt(startX, startY, new Vector2(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x)))
                {
                    PlaceItemAt(itemStack, startX, startY, new Vector2Int(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x), out int _);
                    return true;
                }
            }
            return false;
        }
        public bool AddItemToSlot(ItemSlot itemSlot, int startX, int startY,out int remainingAmount)
        {
            remainingAmount = 0;
            ItemStack itemStack = null;
            if (itemSlot.HasItemStack)
            {
                itemStack = itemSlot.GetItemStack();
            }
            if (itemStack == null)
                return false;

            if (!itemStack.IsRotated)
            {
                if (CanPlaceItemAt(startX, startY, itemStack.ItemData.Size,itemSlot, itemSlot))
                {
                    PlaceItemAt(itemStack, startX, startY, itemStack.ItemData.Size, out remainingAmount);
                    if (remainingAmount > 0)
                    {
                        Debug.Log($"Remaining amount: {remainingAmount}");
                        itemSlot.Container.AddItem(new ItemStack(itemStack.ItemData, false, remainingAmount), out int _);
                    }
                    return true;
                }
            }
            else
            {
                if (CanPlaceItemAt(startX, startY, new Vector2(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x), itemSlot, itemSlot))
                {
                    PlaceItemAt(itemStack, startX, startY, new Vector2Int(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x), out remainingAmount);
                    if (remainingAmount > 0)
                    {
                        AddItem(new ItemStack(itemStack.ItemData, true, remainingAmount), out int _);
                    }
                    return true;
                }
            }
            return false;
        }
        private void PlaceItemAt(ItemStack itemStack, int startX, int startY, Vector2Int itemSize, out int remainingAmount)
        {
            int rootIndex = GetIndexFromGridPos(startX, startY);
            remainingAmount = itemStack.Amount;
            if (itemSlots[rootIndex].HasItemStack)
            {
                if (itemSlots[rootIndex].ItemData.IsStackable)
                {
                    itemSlots[rootIndex].AddAmount(remainingAmount, out remainingAmount);
                    return;
                }
            }
            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    int gridX = startX + x;
                    int gridY = startY + y;

                    int index = gridY * containerWidth + gridX;
                    Vector2Int positionInItem = new Vector2Int(x, y);
                    itemSlots[index].SetItemStack(itemStack, positionInItem, itemSize, rootIndex);
                }
            }

            OnItemPlacedAt?.Invoke(this, new ItemPlacedAtEventArgs
            {
                ItemStack = itemStack,
                RootIndex = rootIndex
            });
        }
        //Ignored slots can be a list later
        public bool CanPlaceItemAt(int startX, int startY, Vector2 itemSize, ItemSlot ignoredSlot = null, ItemSlot targetItem = null)
        {
            //int rootSlot = GetIndexFromGridPos(startX, startY);
            //if (targetItem != null &&itemSlots[rootSlot].ItemData == targetItem.ItemData && targetItem.ItemData.IsStackable)
            //{
            //    return true;
            //}
            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    int gridX = startX + x;
                    int gridY = startY + y;

                    int containerHeight = containerSlots / containerWidth;

                    if (gridX < 0 || gridX >= containerWidth || gridY < 0 || gridY >= containerHeight)
                    {
                        return false;
                    }

                    int index = GetIndexFromGridPos(gridX, gridY);
                    ItemSlot currentSlot = itemSlots[index];

                   

                    if (index < 0 || index >= itemSlots.Count)
                    {
                        return false;
                    }


                    if (ignoredSlot != null && currentSlot.rootIndex == ignoredSlot.rootIndex && currentSlot.Container == ignoredSlot.Container)
                    {
                        continue;
                    }

                    bool hasItemStack = currentSlot.HasItemStack;


                    if (hasItemStack)
                    {
                        if (targetItem != null)
                        {
                            if (!targetItem.ItemData.IsStackable || currentSlot.ItemData != targetItem.ItemData || currentSlot.IsFull || targetItem.IsFull)
                            {
                                return false;
                            }
                            continue;
                        }
                        return false;
                    }
                }
            }
            return true;
        }


        public bool RemoveItem(ItemStack itemStack)
        {
            int amountToRemove = itemStack.Amount;

            foreach (var slot in itemSlots)
            {
                if (slot.HasItemStack && slot.GetItemStack() == itemStack)
                {
                    int amountRemoved = Mathf.Min(amountToRemove, slot.GetItemStack().Amount);
                    itemStack.RemoveAmount(amountRemoved);
                    amountToRemove -= amountRemoved;

                    if (slot.GetItemStack().Amount <= 0)
                    {
                        slot.Clear();
                    }
                    if (amountToRemove <= 0)
                    {
                        return true;
                    }
                }
            }
            return amountToRemove <= 0;
        }
        public bool RemoveItem(int rootIndex)
        {
            ItemSlot rootSlot = itemSlots[rootIndex];

            if (!rootSlot.HasItemStack)
            {
                return false;
            }

            ItemStack itemStack = rootSlot.GetItemStack();
            Vector2Int itemSize = rootSlot.ItemSize;

            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    int gridX = rootSlot.ContainerPosition.x + x;
                    int gridY = rootSlot.ContainerPosition.y + y;

                    int slotIndex = GetIndexFromGridPos(gridX, gridY);
                    ItemSlot slot = itemSlots[slotIndex];
                    if (slot.HasItemStack && slot.GetItemStack().ItemData == itemStack.ItemData)
                    {
                        //Debug.Log($"Slot at position ({gridX}, {gridY}) cleared");
                        slot.Clear();
                    }
                    else
                    {
                        Debug.LogWarning($"Slot at position ({gridX}, {gridY}) does not match the item being removed.");
                    }
                }
            }
            OnItemRootRemoved?.Invoke(this, new ItemRootRemovedEventArgs
            {
                ItemStack = itemStack,
                RootIndex = rootIndex
            });
            return true;
        }

        public void Clear()
        {
            foreach (ItemSlot slot in itemSlots)
            {
                slot.Clear();
            }
        }

        public void Sort()
        {

        }

        public Vector2Int GetGridPos(int index)
        {
            int column = (index % containerWidth);
            int row = Mathf.FloorToInt(index / containerWidth);
            return new Vector2Int(column, row);
        } 
        public int GetIndexFromGridPos(int x, int y)
        {
            return y * containerWidth + x;
        }
    }
    public class Item : MonoBehaviour
    {

    }

    [Serializable]
    public class ItemType
    {
    }

    public class EquippableItemType : ItemType
    {
        public GameObject EquippableItemPrefab;
    }

    public class ConsumableItemType : ItemType
    {
        public float value; //Thinking of value as things like health or energy it will regenerate
    }

    public class DeployableItemType : ItemType
    {
        public GameObject DeployablePrefab;
        public float timeToDeploy;
    }
    public class ItemSlot
    {
        //This worked well
        public int rootIndex;
        private ItemStack itemStack;
        public Container Container { get; private set; }
        public Vector2Int ContainerPosition { get; private set; }
        public int Index { get; private set; }
        public int Amount => itemStack.Amount;
        public ItemData ItemData => itemStack.ItemData;
        public bool HasItemStack => itemStack != null;
        public bool IsFull => itemStack.Amount >= itemStack.ItemData.MaxStackSize;
        public bool IsEmpty => !HasItemStack || itemStack.Amount == 0 || ItemData == null;

        public bool IsRoot => ContainerPosition == Vector2Int.zero ? true : false;

        public event Action<ItemSlot> OnItemAdded;
        public event Action<ItemSlot> OnItemChanged;
        public event Action<ItemSlot> OnItemRemoved;

        public Vector2Int ItemPositionInGrid { get; private set; }
        public Vector2Int ItemSize { get { if (itemStack != null) { if (itemStack.IsRotated) { return new Vector2Int(itemStack.ItemData.Size.y, itemStack.ItemData.Size.x); } else { return itemStack.ItemData.Size; } } return Vector2Int.zero; } }

        public ItemSlot(Container _container, int index, Vector2Int slotPositionOnContainer, ItemStack _itemStack = null)
        {
            Container = _container;
            itemStack = _itemStack;
            Index = index;
            ContainerPosition = slotPositionOnContainer;
        }

        public void SetAmount(int value)
        {

        }
        public void AddAmount(int value, out int remainingAmount)
        {
            int currentAmount = itemStack.Amount;
            int maxStackSize = itemStack.ItemData.MaxStackSize;

            if (currentAmount + value > maxStackSize)
            {
                int amountToAdd = maxStackSize - currentAmount;
                itemStack.AddAmount(amountToAdd);
                remainingAmount = value - amountToAdd;
            }
            else
            {
                remainingAmount = itemStack.AddAmount(value);
            }
            OnItemChanged?.Invoke(this);
        }
        public void RemoveAmount(int value)
        {
            itemStack.RemoveAmount(value);
            OnItemRemoved?.Invoke(this);
        }
        public bool CanAcceptItem(ItemStack itemStack)
        {
            return false;
        }
        public bool CanSwap()
        {
            return false;
        }
        public ItemSlot Clone()
        {
            return null;
        }
        public void SetItemStack(ItemStack _itemStack, Vector2Int itemPositionInGrid, Vector2Int itemSize, int _rootIndex)
        {
            rootIndex = _rootIndex;
            itemStack = _itemStack;
            ItemPositionInGrid = itemPositionInGrid;
            OnItemChanged?.Invoke(this);
        }
        public ItemStack GetItemStack()
        {
            return itemStack;
        }
        public void Clear()
        {
            itemStack = null;
            ItemPositionInGrid = Vector2Int.zero;
            rootIndex = 0;
            OnItemChanged?.Invoke(this);
        }

    }
    [System.Serializable]
    public class ItemStack
    {
        [SerializeField] private ItemData itemData;
        private bool isRotated = false;
        [Min(1)]
        [SerializeField] private int amount = 1;

        public ItemData ItemData => itemData;
        public int Amount => amount;

        public ItemStack()
        {
        }

        public ItemStack(ItemData itemData, bool isRotated = false, int amount = 1)
        {
            this.itemData = itemData;
            this.isRotated = isRotated;
            this.amount = amount;
        }

        public ItemStack(ItemStack other) : this(other.itemData, other.isRotated, other.amount)
        {
        }

        public void SetAmount(int value)
        {
            amount = value;
        }

        public int AddAmount(int value)
        {
            int newAmount = amount += value;
            if (amount > itemData.MaxStackSize)
            {
                int leftover = newAmount - itemData.MaxStackSize;
                amount = itemData.MaxStackSize;
                return leftover;
            }
            amount = newAmount;
            return 0;
        }

        public bool IsRotated => isRotated;

        public void Rotate()
        {
            isRotated = !isRotated;
        }

        public int RemoveAmount(int value)
        {
            amount -= value;
            return amount;
        }

        public ItemStack Clone()
        {
            return new ItemStack(this);
        }

        public void Clear()
        {
            itemData = null;
            amount = 0;
        }
    }
}
