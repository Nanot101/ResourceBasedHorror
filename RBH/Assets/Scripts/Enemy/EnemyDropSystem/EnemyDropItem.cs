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

    public void AddDropForce(Vector2 dropForce)
    {
        if (Time.timeScale > 0.0f)
        {
            rigidbody.AddForce(dropForce);
            return;
        }

        //TODO: If we make a change to the stopping of the game that does not involve a change to the time scale, then remove it.

        var positionOffset = new Vector3(dropForce.x, dropForce.y, 0.0f);
        positionOffset *= 0.005f;

        transform.position += positionOffset;
    }

    private void SetDropNameAndSprite()
    {
        var currentGameObjectName = gameObject.name;
        gameObject.name = currentGameObjectName + " (" + ItemData.name + ")";

        spriteRenderer.sprite = ItemData.ItemSprite;
    }
}