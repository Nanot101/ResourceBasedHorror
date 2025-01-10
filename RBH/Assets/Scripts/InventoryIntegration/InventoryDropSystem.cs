using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

public class InventoryDropSystem : MonoBehaviour
{
    [SerializeField] private float dropForce = 100.0f;

    [SerializeField] private EnemyDropItem dropItemPrefab;

    private void Start()
    {
        Debug.Assert(dropItemPrefab != null, $"{nameof(dropItemPrefab)} is required for {nameof(InventoryDropSystem)}",
            this);
    }

    public EnemyDropItem DropItem(ItemData itemData, List<ItemStack> itemStacks, Vector3 position)
    {
        var dropItem = Instantiate(dropItemPrefab, position, Quaternion.identity);

        dropItem.Init(itemData, itemStacks);

        dropItem.AddDropForce(GetRandomDropForce());

        return dropItem;
    }

    private Vector2 GetRandomDropForce()
    {
        var randomDirection = GetRandomDirection();

        var randomForce = dropForce + Random.Range(-15.0f, 15.0f);

        return randomDirection * randomForce;
    }

    private static Vector2 GetRandomDirection()
    {
        var randomDirection = RandomInsideUnitCircleNormalized();

        while (randomDirection.magnitude == 0.0f)
        {
            randomDirection = RandomInsideUnitCircleNormalized();
        }

        return randomDirection;

        static Vector2 RandomInsideUnitCircleNormalized() => Random.insideUnitCircle.normalized;
    }
}