using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int Height { get { if (rotated == false) { return itemData.height; } return itemData.width; } }
    public int Width
    {
        get { if (rotated == false) { return itemData.width; } return itemData.height; }
    }

    public int gridPositionX;
    public int gridPositionY;

    public bool rotated = false;

    public void Set(ItemData itemData)
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileWidth ;
        size.y = itemData.height * ItemGrid.tileHeight;
        GetComponent<RectTransform>().sizeDelta = size;
        GetComponent<RectTransform>().localScale *= transform.root.GetComponentInChildren<RectTransform>().localScale.x;
    }
    public void SetCustomSize(Vector2 customSize)
    {
        Vector2 size = new Vector2();
        size.x = customSize.x * ItemGrid.tileWidth;
        size.y = customSize.y * ItemGrid.tileHeight;
        GetComponent<RectTransform>().sizeDelta = size;
        GetComponent<RectTransform>().localScale *= transform.root.GetComponentInChildren<RectTransform>().localScale.x;
    }
    public void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated ? 90 : 0);

    }
}
