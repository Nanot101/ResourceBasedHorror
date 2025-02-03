using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class ContextMenuController : MonoBehaviour
    {
        [SerializeField] private InventorySystem system;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private ContextMenu menuPrefab;
        [SerializeField] private Transform slotContent;
        private ContextMenu menuInstance;
        private GridContainerView currentTargetGridContainerView;
        private SlotView currentSlotView;
        private SlotView targetSlotView;
        [SerializeField] bool isMouseOverMenuInstance;
        //This update will need to be brought somewhere else to separate input logic, right now it'll be doing the shortcuts here
        private void Update()
        {
            if (!system.IsOpen)
            {
                return;
            }
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerData, results);
            bool currentLoopHasMenu = false;
            foreach (var result in results)
            {
                if (result.gameObject.CompareTag("ContextMenu"))
                {
                    currentLoopHasMenu = true;
                }
                var slotView = result.gameObject.GetComponent<SlotView>();
                if (slotView != null)
                {
                    currentSlotView = slotView;
                    break;
                }
            }
            if (currentLoopHasMenu)
            {
                isMouseOverMenuInstance = true;
            }
            else
            {
                isMouseOverMenuInstance = false;
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Input.GetKey(KeyCode.LeftShift)&&currentSlotView.itemSlot.HasItemStack)
                {
                    var desiredInventory = system.GetContainerGridInPosition(InventorySystem.InventoryPositionType.TemporaryInventory);
                    if (!desiredInventory||currentSlotView.GridContainerView == desiredInventory )
                    {
                        desiredInventory = system.GetContainerGridInPosition(InventorySystem.InventoryPositionType.PlayerInventory);
                    }
                    if (desiredInventory != null)
                    {
                        desiredInventory.container.AddItem(currentSlotView.rootSlotView.itemSlot.GetItemStack(),out int asss);
                        currentSlotView.GridContainerView.container.RemoveItem(currentSlotView.itemSlot.rootIndex);
                        currentSlotView.rootSlotView.DestroyItemHighlight();
                    }
                }
                if (menuInstance != null && !isMouseOverMenuInstance)
                    menuInstance.Close();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                targetSlotView = currentSlotView;
                if (menuInstance == null)
                {
                    menuInstance = Instantiate(menuPrefab, graphicRaycaster.transform);
                }
                if (menuInstance != null)
                {
                    menuInstance.Close();
                }
                if (!targetSlotView)
                {
                    menuInstance.Close();
                    return;
                }
                if (targetSlotView.isEmpty)
                {
                    menuInstance.Close();
                    return;
                }
                currentTargetGridContainerView = targetSlotView.GridContainerView;
                Vector3 offset = new Vector3(-60, -60, 0);
                menuInstance.transform.position = Input.mousePosition + offset;
                menuInstance.Initialize(targetSlotView.itemSlot);
                menuInstance.Open();

            }

        }
        public void CloseContextMenuInstance(GridContainerView gridContainerView)
        {
            if (menuInstance != null && gridContainerView == currentTargetGridContainerView)
            {
                menuInstance.Close();
                currentTargetGridContainerView = null;
            }
        }

    }
}
