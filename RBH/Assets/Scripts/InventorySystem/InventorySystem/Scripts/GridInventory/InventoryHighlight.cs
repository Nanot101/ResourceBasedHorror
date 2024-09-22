using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;

    public void Show (bool value)
    {
        highlighter.gameObject.SetActive(value);
    }

    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.Width * ItemGrid.tileWidth;
        size.y = targetItem.Height * ItemGrid.tileHeight;
        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.gridPositionX, targetItem.gridPositionY);

        highlighter.localPosition = pos;

    }

    public void SetParent(ItemGrid targetGrid)
    {
        if (targetGrid != null)
            highlighter.SetParent(targetGrid.transform);
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }
}
