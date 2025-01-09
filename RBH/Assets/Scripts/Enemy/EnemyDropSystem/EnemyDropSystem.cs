using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class EnemyDropSystem : MonoBehaviour
{
    [SerializeField] private float dropForce = 100.0f;

    [SerializeField] private EnemyDropItem dropItemPrefab;

    private readonly List<EnemyDropSO> enemyDropSOs = new();

    void Start()
    {
        Debug.Assert(dropItemPrefab != null, $"{nameof(dropItemPrefab)} is required for {nameof(EnemyDropSystem)}",
            this);
    }

    public void SetDrops(IEnumerable<EnemyDropSO> drops)
    {
        enemyDropSOs.Clear();
        enemyDropSOs.AddRange(drops);
    }

    public void DropItems()
    {
        if (enemyDropSOs.Count == 0)
        {
            return;
        }

        var dropDirection = GetRandomInitialDropDirection();
        var directionAngleStep = 360.0f / enemyDropSOs.Count;

        foreach (var dropSO in enemyDropSOs)
        {
            DropItem(dropSO, dropDirection);

            AddDirectionStep();
        }

        return;

        void AddDirectionStep()
        {
            var randomStep = directionAngleStep + Random.Range(-10.0f, 10.0f);

            dropDirection = Quaternion.AngleAxis(randomStep, Vector3.forward) * dropDirection;
        }
    }

    private void DropItem(EnemyDropSO dropSO, Vector2 dropDir)
    {
        var dropItem = Instantiate(dropItemPrefab, transform.position, transform.rotation);

        dropItem.Init(dropSO.DropItemData, CreateInitialItemStacks(dropSO));

        var randomDropForce = dropForce + Random.Range(-15.0f, 15.0f);
        dropItem.AddDropForce(dropDir * randomDropForce);
    }

    private static Vector2 GetRandomInitialDropDirection()
    {
        var randomDirection = RandomInsideUnitCircleNormalized();

        while (randomDirection.magnitude == 0.0f)
        {
            randomDirection = RandomInsideUnitCircleNormalized();
        }

        return randomDirection;

        static Vector2 RandomInsideUnitCircleNormalized() => Random.insideUnitCircle.normalized;
    }

    private static List<ItemStack> CreateInitialItemStacks(EnemyDropSO dropSO)
    {
        var itemMaxStack = dropSO.DropItemData.MaxStackSize;
        var itemAmount = Random.Range(dropSO.MinQuantity, dropSO.MaxQuantity + 1);

        var itemStacks = new List<ItemStack>();

        while (itemAmount > 0)
        {
            var newItemStackAmount = Mathf.Min(itemAmount, itemMaxStack);

            var newItemStack = new ItemStack(dropSO.DropItemData, amount: newItemStackAmount);
            itemStacks.Add(newItemStack);

            itemAmount -= newItemStackAmount;
        }

        return itemStacks;
    }
}