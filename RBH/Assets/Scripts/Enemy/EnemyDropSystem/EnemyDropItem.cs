using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDropItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private new Rigidbody2D rigidbody;

    [SerializeField] private float lifetimeInSeconds = 60.0f;

    private ItemData itemData;
    private List<ItemStack> itemStacks;

    public ItemData ItemData
    {
        get
        {
            RefreshLifetime();
            return itemData;
        }
        private set { itemData = value; }
    }

    public List<ItemStack> ItemStacks
    {
        get
        {
            RefreshLifetime();
            return itemStacks;
        }
        private set { itemStacks = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spriteRenderer != null, $"{nameof(spriteRenderer)} is required for {nameof(EnemyDropItem)}", this);
        Debug.Assert(rigidbody != null, $"{nameof(rigidbody)} is required for {nameof(EnemyDropItem)}", this);

        StartCoroutine(DestroyCoroutine());
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

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(lifetimeInSeconds);

        Destroy(gameObject);
    }

    private void RefreshLifetime()
    {
        StopAllCoroutines();

        StartCoroutine(DestroyCoroutine());
    }
}