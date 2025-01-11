using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ContextMenuButton : MonoBehaviour
    {
        [SerializeField] Button m_Button;
        [SerializeField] TextMeshProUGUI buttonText;
        ItemSlot itemSlot;
        ItemAction action;

        public void Initialize(ItemAction _action, ItemSlot _itemSlot)
        {
            action = _action;
            itemSlot = _itemSlot;
            buttonText.text = action.ActionName;
            m_Button.onClick.AddListener(OnClick);
        }

        public void OnClick()
        {
            action.Execute(itemSlot);
        }
    }
}
