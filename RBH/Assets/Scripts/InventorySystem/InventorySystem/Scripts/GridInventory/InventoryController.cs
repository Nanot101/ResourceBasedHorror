using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(ItemDatabaseRetriever))]
public class InventoryController : MonoBehaviour
{
    private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid { get { return selectedItemGrid; } set { selectedItemGrid = value; inventoryHighlight.SetParent(value); } }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    ItemDatabase itemDatabase;
    [SerializeField] InventoryItem itemPrefab;
    [SerializeField] Transform canvasTransform;

    [SerializeField] GameObject interactableInventoryTab;

    InventoryHighlight inventoryHighlight;


    Vector2Int oldPosition;

    InventoryItem itemToHighlight;


    private void Awake()
    {
        itemDatabase = GetComponent<ItemDatabaseRetriever>().itemDatabase;
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }
    private void Start()
    {
        CloseInventory();
    }
    private void Update()
    {
        ItemIconDrag();
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (selectedItem != null)
                return;
            InsertRandomItem();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RemoveSelectedItem();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (selectedItem != null)
            {
                Debug.Log("Item is selected, can't close inventory");
                return;
            }
            selectedItemGrid = null;
            if (interactableInventoryTab.gameObject.activeSelf)
                CloseInventory();
            else
                OpenInventory();
        }
        if (selectedItemGrid == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            LeftMouseButtonPress();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            RightMouseButtonPress();
        }
    }

    private void OpenInventory()
    {
        interactableInventoryTab.SetActive(true);
    }
    private void CloseInventory()
    {
        interactableInventoryTab.SetActive(false);
    }

    private void RotateItem()
    {
        if (selectedItem == null)
            return;

        selectedItem.Rotate();
    }

    private void InsertRandomItem()
    {
        if (selectedItemGrid == null)
            return;

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        bool complete = InsertItem(itemToInsert);
        if (complete)
            return;
        Destroy(itemToInsert.gameObject);
    }

    private bool InsertItem(InventoryItem itemToInsert)
    {
        if (selectedItemGrid == null)
        {
            Debug.LogError("No grid selected");
            return false;
        }

        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null)
        {
            Debug.LogError("No space found for item");
            return false;
        }
        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }
    public bool InsertItem(ItemData itemToInsert, ItemGrid targetGrid)
    {
        if (targetGrid == null)
        {
            Debug.LogError("No grid selected");
            return false;
        }
        InventoryItem inventoryItemToInsert = CreateItem(itemToInsert, targetGrid);
        if (inventoryItemToInsert == null)
        {
            Debug.LogError("Something went wrong, Item not created");
            return false;
        }
        Vector2Int? posOnGrid = targetGrid.FindSpaceForObject(inventoryItemToInsert);

        if (posOnGrid == null)
        {
            Debug.LogError("No space found for item");
            return false;
        }
        targetGrid.PlaceItem(inventoryItemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        if (oldPosition == positionOnGrid)
            return;

        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if (itemToHighlight == null)
            {
                inventoryHighlight.Show(false);
                return;
            }
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(itemToHighlight);
            //inventoryHighlight.SetParent(selectedItemGrid);
            inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.Width, selectedItem.Height));
            inventoryHighlight.SetSize(selectedItem);
            //inventoryHighlight.SetParent(selectedItemGrid);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab);
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        int selectedItemID = UnityEngine.Random.Range(0, itemDatabase.items.Count());
        inventoryItem.Set(itemDatabase.items[selectedItemID]);
        inventoryItem.gameObject.name = inventoryItem.itemData.itemName;
    }

    private InventoryItem CreateItem(ItemData item, ItemGrid targetGrid)
    {
        ItemData foundItem = itemDatabase.FindItemInDatabase(item.id);
        if (foundItem == null)
        {
            Debug.LogError("Item not found in database");
            return null;
        }

        InventoryItem inventoryItem = Instantiate(itemPrefab);
        RectTransform itemRect = inventoryItem.GetComponent<RectTransform>();
        itemRect.SetParent(canvasTransform);
        inventoryItem.Set(foundItem);
        inventoryItem.gameObject.name = inventoryItem.itemData.itemName;

        return inventoryItem;

    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private void RightMouseButtonPress()
    {
        Vector2Int tileGridPosition = selectedItemGrid.GetTileGridPosition(Input.mousePosition);
        InventoryItem item = selectedItemGrid.GetItem(tileGridPosition.x, tileGridPosition.y);
        selectedItemGrid.RemoveItem(item, tileGridPosition.x, tileGridPosition.y);
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.Width - 1) * ItemGrid.tileWidth / 2;
            position.y += (selectedItem.Height - 1) * ItemGrid.tileHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete)
        {
            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                SetItemToFrontOfHierarchy();
                overlapItem = null;
            }
        }
    }

    private void SetItemToFrontOfHierarchy()
    {
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
            SetItemToFrontOfHierarchy();
        }
    }
    private void RemoveSelectedItem()
    {
        if (selectedItem != null)
        {
            Destroy(selectedItem.gameObject);
            selectedItem = null;
        }
    }

    public void RemoveItem(ItemData item, int amount, ItemGrid targetGrid)
    {
        List<InventoryItem> invItem = targetGrid.GetItems(item);
        if (invItem == null || invItem.Count == 0)
        {
            Debug.LogError("Item not found in grid");
            return;
        }
        int amountToDestroy = amount;
        for (int i = 0; i < invItem.Count; i++)
        {
            if (amountToDestroy == 0)
                return;
            targetGrid.RemoveItem(invItem[i], invItem[i].gridPositionX, invItem[i].gridPositionY);
            amountToDestroy--;
        }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }
}
