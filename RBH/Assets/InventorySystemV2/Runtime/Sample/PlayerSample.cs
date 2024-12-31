using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    [RequireComponent(typeof(ContainerHandler))]
    public class PlayerSample : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private InventorySystem system;
        [SerializeField] private ContainerHandler myInventoryContainer;
        [SerializeField] private GridContainerView containerView;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private ContextMenu menuPrefab;
        private ContextMenu menuInstance;
        private SlotView currentSlotView;
        private SlotView targetSlotView;
        [SerializeField] bool isMouseOverMenuInstance;
        public KeyCode keyToOpenInventory = KeyCode.Tab;
        private void Start()
        {
            containerView = system.CreateOrGetContainerGridInPosition(myInventoryContainer.Container,InventorySystem.InventoryPositionType.PlayerInventory);
        }
        private void Update()
        {
            if (Input.GetKeyDown(keyToOpenInventory))
            {
                containerView.ToggleContainer(myInventoryContainer.Container);
            }
            if (!containerView.IsVisible)
                return;
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
                        ContextMenu menuInstance = Instantiate(menuPrefab, containerView.SlotContent.parent);
                        menuInstance.transform.position = Input.mousePosition;
                        menuInstance.Initialize(slotview.itemSlot);
                        Debug.Log("Welcome");
                    }
                }
            }
        }
    }
}
