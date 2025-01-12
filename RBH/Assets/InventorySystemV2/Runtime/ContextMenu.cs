using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class ContextMenu : MonoBehaviour
    {
        private List<ContextMenuButton> buttons = new List<ContextMenuButton>();

        [SerializeField] private ContextMenuButton menuButtonPrefab;

        private void OnEnable()
        {
            ItemAction.actionExecuted += Close;
        }
        private void OnDisable()
        {
            ItemAction.actionExecuted -= Close;
        }

        public void Initialize(ItemSlot targetSlot)
        {
            if (targetSlot.IsEmpty)
            {
                Close();
                return;
            }
                
            ItemAction[] itemActions = targetSlot.ItemData.ItemAction;
            for (int i = 0; i < itemActions.Length; i++)
            {
                ContextMenuButton button = Instantiate(menuButtonPrefab, transform);
                button.Initialize(itemActions[i],targetSlot);
                buttons.Add(button);
            }
        }
        public void Open()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            buttons.DestroyAllObjectsInList();
            buttons.Clear();
            gameObject.SetActive(false);

        }
    }
}
