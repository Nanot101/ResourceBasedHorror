using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI buttonText;
        ItemSlot itemSlot;
        ItemAction action;

        public void Initialize(ItemAction _action, ItemSlot _itemSlot)
        {
            action = _action;
            itemSlot = _itemSlot;
            buttonText.text = action.ActionName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                action.Execute(itemSlot);
        }
    }
}
