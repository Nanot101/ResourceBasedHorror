using System;
using System.Collections;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ItemGrid craftResultsGrid;
    private InventoryController inventoryController;
    [SerializeField] private Recipe selectedItem;

    private SelectSystem selectSystem;

    public Action OnCraft;

    private void OnEnable()
    {
       
        SelectSystem.OnSelection += OnSelection;
    }

    private void OnDisable()
    {
        SelectSystem.OnSelection -= OnSelection;
    }

    private void Awake()
    {
        selectSystem = GetComponent<SelectSystem>();
        inventoryController = playerInventory.GetComponent<InventoryController>();
    }

    private void OnSelection(Selectables selectables, SelectSystem s)
    {
        selectedItem = (Recipe)selectables.selection;
    }

    public void TryCraft()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("No item selected");
            return;
        }
        if (selectedItem.craftingRecipe.ingredients.Length == 0)
        {
            //Intended behavior -> there should be atleast one ingredient for the player to use for crafting 
            Debug.LogError("The recipe "+ selectedItem.itemData.itemName + " has no ingredients! ");
            return;
        }
        if (!CanCraft(selectedItem))
            return;

        Craft();
        OnCraft?.Invoke();

    }

    private void Craft()
    {
        RemoveItems();
        inventoryController.InsertItem(selectedItem.itemData, craftResultsGrid);
    }

    public bool CanCraft(Recipe item)
    {
        for (int i = 0; i < item.craftingRecipe.ingredients.Length; i++)
        {
            //Check for item amount in player's inventory
            if (playerInventory.GetItemAmount(item.craftingRecipe.ingredients[i].item) < item.craftingRecipe.ingredients[i].amount)
            {
                return false;
            }
        }
        return true;
    }

    private void RemoveItems()
    {
        for (int i = 0; i < selectedItem.craftingRecipe.ingredients.Length; i++)
        {
            //Remove item from player's inventory
            inventoryController.RemoveItem(selectedItem.craftingRecipe.ingredients[i].item, selectedItem.craftingRecipe.ingredients[i].amount,craftResultsGrid);
        }
    }

}
