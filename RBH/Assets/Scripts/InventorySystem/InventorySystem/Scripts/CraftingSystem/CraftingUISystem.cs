using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(SelectSystem))]
[RequireComponent(typeof(CraftingSystem))]
public class CraftingUISystem : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Recipe selectedItem;
    [SerializeField] private GameObject craftingIngredient;
    [SerializeField] private Transform craftingIngredientParent;
    [SerializeField] private Button craftButton;
    private List<CraftingIngredientUIPrefab> craftingIngredients = new List<CraftingIngredientUIPrefab>();

    private SelectSystem selectSystem;
    private CraftingSystem craftingSystem;

    private void OnEnable()
    {
        
        SelectSystem.OnSelection += OnSelection;
        //I can bring the update text logic to here, it will create ingredients everytime but it only happens twice at max and not many ingredients to begin, with but will centralize the logic
        craftingSystem.OnCraft += DisplayCraftingUI;
        DisplayCraftingUI();
    }


    private void OnDisable()
    {
        SelectSystem.OnSelection -= OnSelection;
        craftingSystem.OnCraft -= DisplayCraftingUI;
    }

    private void Awake()
    {
        craftingSystem = GetComponent<CraftingSystem>();
        selectSystem = GetComponent<SelectSystem>();
    }

    private void Start()
    {
        
        craftButton.onClick.AddListener(() => craftingSystem.TryCraft());
        craftButton.onClick.AddListener(CanInteract);
    }
    public void DisplayCraftingUI()
    {
        craftingIngredientParent.DestroyAllChildren();
        if (selectedItem != null)
        {
            for (int i = 0; i < selectedItem.craftingRecipe.ingredients.Length; i++)
            {
                CraftingIngredientUIPrefab craftIngredientInstance = Instantiate(craftingIngredient, craftingIngredientParent).GetComponent<CraftingIngredientUIPrefab>();
                craftingIngredients.Add(craftIngredientInstance);
                CraftingIngredients recipe = selectedItem.craftingRecipe.ingredients[i];
                if (recipe == null || recipe.item == null)
                {
                    Debug.LogError("Item is null");
                    return;
                }
                craftIngredientInstance.Initialize(recipe.item, recipe.amount,playerInventory);
            }
        }
    }
    private void OnSelection(Selectables selectables, SelectSystem select)
    {

        selectedItem = (Recipe)selectables.selection;
        DisplayCraftingUI();
        CanInteract();
    }

    private void CanInteract()
    {
        if (!craftingSystem.CanCraft(selectedItem))
        {
            craftButton.interactable = false;
            return;
        }
        
        //if (playerInventory.HasItem(selectedItem.itemData))
        //{
        //    craftButton.interactable = false;
        //    return;
        //}
        craftButton.interactable = true;
    }
}
