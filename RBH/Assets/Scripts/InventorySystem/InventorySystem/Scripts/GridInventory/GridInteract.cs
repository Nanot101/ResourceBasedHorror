using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] InventoryController controller;
    ItemGrid itemGrid;

    public void OnPointerEnter(PointerEventData eventData)
    {
        controller.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        controller.SelectedItemGrid = null;
    }

    private void Awake()
    {
        itemGrid = GetComponent<ItemGrid>();
    }

}
