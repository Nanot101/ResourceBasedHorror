using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGrid : MonoBehaviour
{
    public RectTransform canvasScaler;
    [Header("Tile")]
    public static float tileWidth = 64;
    public static float tileHeight = 64;

    [Header("Grid")]
    [SerializeField] int gridSizeWidth = 8;
    [SerializeField] int gridSizeHeight = 8;

    InventoryItem[,] inventoryItemSlot;
    [SerializeField] Inventory inventory;

    [SerializeField] InventoryItem inventoryItemPrefab;

    RectTransform rectTransform;
    Vector2 gridPosition = new Vector2();
    Vector2Int gridTilePosition = new Vector2Int();

    private void Awake()
    {
        if (inventory == null)
            inventory = gameObject.AddComponent<Inventory>();
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem inventoryItem = inventoryItemSlot[x, y];
        if (inventoryItem == null)
            return null;
        RemoveItemReference(inventoryItem);
        return inventoryItem;
    }

    private void ClearGridReference(InventoryItem inventoryItem, float width, float height)
    {
        for (int ix = 0; ix < width; ix++)
        {
            for (int iy = 0; iy < height; iy++)
            {
                inventoryItemSlot[inventoryItem.gridPositionX + ix, inventoryItem.gridPositionY + iy] = null;
            }
        }
    }

    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileWidth, height * tileHeight);
        rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        //Vector2 localMousePos = mousePosition / canvasScaler.localScale.x;
        Vector2 localMousePos = mousePosition;
        gridPosition.x = localMousePos.x - rectTransform.position.x;
        gridPosition.y = rectTransform.position.y - localMousePos.y;

        gridTilePosition.x = (int)(gridPosition.x / (tileWidth * canvasScaler.localScale.x));
        gridTilePosition.y = (int)(gridPosition.y / (tileHeight * canvasScaler.localScale.x));
        return gridTilePosition;
    }

    public virtual bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        int width = inventoryItem.Width;
        int height = inventoryItem.Height;

        if (BoundryCheck(posX, posY, width, height) == false)
            return false;

        if (OverlapCheck(posX, posY, width, height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            RemoveItemReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);
        return true;
    }

    public bool RemoveItem(InventoryItem inventoryItem, int posX, int posY)
    {
        if (inventoryItem != null)
        {
            RemoveItemReference(inventoryItem);
            Destroy(inventoryItem.gameObject);
            return true;
        }
        Debug.Log("Failed to remove item");
        return false;
    }

    public void RemoveItemReference(InventoryItem inventoryItem)
    {
        inventory.RemoveItem(inventoryItem.itemData, 1);
        ClearGridReference(inventoryItem, inventoryItem.Width, inventoryItem.Height);
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform itemRectTransform = inventoryItem.GetComponent<RectTransform>();
        itemRectTransform.SetParent(rectTransform);
        float width = inventoryItem.Width;
        float height = inventoryItem.Height;

        inventory.AddItem(inventoryItem.itemData, 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.gridPositionX = posX;
        inventoryItem.gridPositionY = posY;

        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        itemRectTransform.localPosition = position;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        float scaledTileWidth = tileWidth;
        float scaledTileHeight = tileHeight;
        Vector2 position = new Vector2();
        position.x = posX * scaledTileWidth + scaledTileWidth * inventoryItem.Width / 2;
        position.y = -(posY * scaledTileHeight + scaledTileHeight * inventoryItem.Height / 2);
        return position;
    }
    protected bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool PositionCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0)
        {
            return false;
        }

        if (posX >= gridSizeWidth || posY >= gridSizeHeight)
        {
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if (PositionCheck(posX, posY) == false)
            return false;

        posX += width - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false)
            return false;

        return true;
    }

    public InventoryItem GetItem(float x, float y)
    {
        return inventoryItemSlot[(int)x, (int)y];
    }

    public List<InventoryItem> GetItems(ItemData item)
    {
        List<InventoryItem> items = new List<InventoryItem>();
        for (int x = 0; x < gridSizeWidth; x++)
        {
            for (int y = 0; y < gridSizeHeight; y++)
            {
                if (inventoryItemSlot[x, y] != null && inventoryItemSlot[x, y].itemData == item)
                {
                    items.Add(inventoryItemSlot[x, y]);
                }
            }
        }
        return items;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        if (itemToInsert == null)
            return null;
        int height = gridSizeHeight - itemToInsert.Height + 1;
        int width = gridSizeWidth - itemToInsert.Width + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.Width, itemToInsert.Height))
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }
}
