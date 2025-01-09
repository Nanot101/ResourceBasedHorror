using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDropItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private new Rigidbody2D rigidbody;

    public ItemData ItemData { get; private set; }

    public List<ItemStack> ItemStacks { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spriteRenderer != null, $"{nameof(spriteRenderer)} is required for {nameof(EnemyDropItem)}", this);
        Debug.Assert(rigidbody != null, $"{nameof(rigidbody)} is required for {nameof(EnemyDropItem)}", this);
    }

    public void Init(ItemData itemData, List<ItemStack> itemStacks)
    {
        ItemData = itemData;
        ItemStacks = itemStacks;

        SetDropNameAndSprite();
    }

    public void AddDropForce(Vector2 dropForce) => rigidbody.AddForce(dropForce);

    private void SetDropNameAndSprite()
    {
        var currentGameObjectName = gameObject.name;
        gameObject.name = currentGameObjectName + " (" + ItemData.name + ")";

        spriteRenderer.sprite = ItemData.ItemSprite;
    }
}