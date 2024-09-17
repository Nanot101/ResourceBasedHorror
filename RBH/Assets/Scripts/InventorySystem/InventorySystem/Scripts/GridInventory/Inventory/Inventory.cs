using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Inventory : MonoBehaviour
{
    public Dictionary<ItemData, int> itemSlot = new Dictionary<ItemData, int>();

    public Action<ItemData, int> OnItemAmountChanged;


    public void AddItem(ItemData item, int amount)
    {

        //create a new inventory item by trying to place it via the inventory controller

        if (item == null)
        {
            Debug.LogError("Item is null");
            return;
        }
        if (itemSlot.ContainsKey(item))
        {
            itemSlot[item] += amount;
        }
        else
        {
            itemSlot.Add(item, amount);
        }
        OnItemAmountChanged?.Invoke(item, amount);
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        //Remove the item or items from the inventory

        if (itemSlot.ContainsKey(item))
        {
            if (itemSlot[item] >= amount)
            {
                itemSlot[item] -= amount;
                if (itemSlot[item] == 0)
                {
                    itemSlot.Remove(item);
                }
                OnItemAmountChanged?.Invoke(item, amount);
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    public int GetItemAmount(ItemData item)
    {
        return HasItem(item) ? itemSlot[item] : 0;
    }
    public bool HasItem(ItemData item)
    {
        return itemSlot.ContainsKey(item);
    }
}
