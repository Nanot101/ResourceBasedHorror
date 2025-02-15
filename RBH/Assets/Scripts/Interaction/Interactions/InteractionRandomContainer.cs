using InventorySystem;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRandomContainer : InteractionContainerBase
{
    [SerializeField] List<WeightedItem> weightedLootTable;
    [SerializeField] private int itemAmountMin = 6;
    [SerializeField] private int itemAmountMax = 20;
    protected override void Start()
    {
        base.Start();
        int amount = Random.Range(itemAmountMin, itemAmountMax);
        for (int i = 0; i < amount; i++)
        {
            ItemData randomItem = GetRandomItem();
            containerHandler.Container.AddItem(new ItemStack(randomItem), out _);
        }
    }
    public override void Interact(IInteractionCaller caller)
    {
        //Repeatable for testing
        //containerHandler.Container.Clear();
        //int amount = Random.Range(itemAmountMin, itemAmountMax);
        //for (int i = 0; i < amount; i++)
        //{
        //    ItemData randomItem = GetRandomItem();
        //    containerHandler.Container.AddItem(new ItemStack(randomItem), out _);
        //}
        base.Interact(caller);
    }
    private ItemData GetRandomItem()
    {
        float totalWeight = 0f;
        foreach (var wi in weightedLootTable)
        {
            totalWeight += wi.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (var wi in weightedLootTable)
        {
            randomValue -= wi.weight;
            if (randomValue <= 0f)
            {
                return wi.item;
            }
        }

        //fallback
        return weightedLootTable[weightedLootTable.Count - 1].item;
    }
}
[System.Serializable]
public class WeightedItem
{
    public ItemData item;
    public float weight;
}