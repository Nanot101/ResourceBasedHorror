using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class Player : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ContainerHandler myInventoryContainer;
        [SerializeField] private ContainerView containerView;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Menu menuPrefab;
        private Menu menuInstance;
        private SlotView currentSlotView;
        private SlotView targetSlotView;
        [SerializeField] bool isMouseOverMenuInstance;
        public KeyCode keyToOpenInventory = KeyCode.Tab;
        private void Update()
        {
            if (Input.GetKeyDown(keyToOpenInventory))
            {
                if (containerView.IsVisible)
                    containerView.HideContainer();
                else
                    containerView.ShowContainer(myInventoryContainer.Container);
            }
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerData, results);
            bool currentLoopHasMenu = false;
            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Menu>() || result.gameObject.GetComponent<MenuButton>())
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
                if (menuInstance != null && !isMouseOverMenuInstance)
                    menuInstance.Close();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (menuInstance != null)
                {
                    menuInstance.Close();
                }
                targetSlotView = currentSlotView;
                Debug.Log("Right-clicked on an Slot View!");
                menuInstance = Instantiate(menuPrefab, graphicRaycaster.transform);
                Vector3 offset = new Vector3(-60, -60, 0);
                menuInstance.transform.position = Input.mousePosition + offset;
                menuInstance.Initialize(targetSlotView.itemSlot);
            }

        }

        public void AddItem(int index)
        {
            myInventoryContainer.Container.AddItem(myInventoryContainer.initialItems[index], out int _);
        }
        private void RemoveItem(ItemStack itemStack)
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Hello1");
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
                if (clickedObject != null)
                {
                    SlotView slotview = clickedObject.GetComponent<SlotView>();
                    if (slotview != null)
                    {
                        Menu menuInstance = Instantiate(menuPrefab, containerView.SlotContent.parent);
                        menuInstance.transform.position = Input.mousePosition;
                        menuInstance.Initialize(slotview.itemSlot);
                        Debug.Log("Welcome");
                    }
                }
            }
        }
    }
}
