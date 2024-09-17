using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingIngredientUIPrefab : MonoBehaviour
{
    private Inventory playerInventory;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] Color missingIngredientsColor;

    private ItemData item;
    private int desiredAmount;

    private void OnEnable()
    {
        //Subscribe to changes in the inventory to update the amount of the item
        //PlayerInventory.Instance.inventory.OnItemAmountChanged += UpdateAmount;
        //this.playerInventory.OnItemAmountChanged += UpdateAmount;
    }

    private void OnDisable()
    {
        playerInventory.OnItemAmountChanged -= UpdateAmount;
    }

    private void UpdateAmount(ItemData data, int amount)
    {
        if (data == item)
        {
            float itemAmount = playerInventory.GetItemAmount(item);
            //float itemAmount = 0;

            this.amountText.text = itemAmount.ToString() + "/" + desiredAmount;
            amountText.color = itemAmount >= desiredAmount ? Color.white : missingIngredientsColor;
        }
    }

    public void Initialize(ItemData item, int necessaryAmount, Inventory playerInventory)
    {
        this.icon.sprite = item.itemIcon;
        this.nameText.text = item.itemName;

        this.playerInventory = playerInventory;
        float itemAmount = this.playerInventory.GetItemAmount(item);
        this.playerInventory.OnItemAmountChanged += UpdateAmount;
        //float itemAmount = 0;

        this.amountText.text = itemAmount.ToString() + "/" + necessaryAmount;//amount of item required
        //this.nameText.color =  itemAmount >= necessaryAmount ? Color.white : missingIngredientsColor;
        this.amountText.color = itemAmount >= necessaryAmount ? Color.white : missingIngredientsColor;
        desiredAmount = necessaryAmount;
        this.item = item;
    }


}
