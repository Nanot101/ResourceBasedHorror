using System;
using System.Collections.Generic;
using InventorySystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using InvSys = InventorySystem.InventorySystem;
using CtxMenu = InventorySystem.ContextMenu;
using InvPos = InventorySystem.InventorySystem.InventoryPositionType;

[RequireComponent(typeof(ContainerHandler))]
public class InventoryViewController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InvSys inventorySystem;
    [SerializeField] private ContainerHandler playerInventoryContainer;
    [SerializeField] private CtxMenu menuPrefab;

    [SerializeField] private GraphicRaycaster graphicRaycaster;

    [field: SerializeField] public KeyCode KeyToOpenInventory { get; private set; } = KeyCode.Tab;

    private GridContainerView playerContainerView;
    private GridContainerView tempContainerView;

    private Container tempInventoryContainer;

    private CtxMenu menuInstance;
    private SlotView currentSlotView;
    private SlotView targetSlotView;

    private void Start()
    {
        AssertDesignerFields();

        playerContainerView = inventorySystem.CreateOrGetContainerGridInPosition(playerInventoryContainer.Container,
            InvPos.PlayerInventory);
    }

    private void Update()
    {
        if (!playerContainerView.IsVisible)
        {
            return;
        }

        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        var currentLoopHasMenu = false;

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("ContextMenu"))
            {
                currentLoopHasMenu = true;
            }

            var slotView = result.gameObject.GetComponent<SlotView>();

            if (!slotView)
            {
                continue;
            }

            currentSlotView = slotView;
            break;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && menuInstance && !currentLoopHasMenu)
        {
            menuInstance.Close();
        }

        if (!Input.GetKeyDown(KeyCode.Mouse1))
        {
            return;
        }

        if (menuInstance)
        {
            menuInstance.Close();
        }

        targetSlotView = currentSlotView;

        menuInstance = Instantiate(menuPrefab, graphicRaycaster.transform);

        var offset = new Vector3(-60, -60, 0);
        menuInstance.transform.position = Input.mousePosition + offset;

        menuInstance.Initialize(targetSlotView.itemSlot);
    }

    public void ShowPlayerInventory()
    {
        if (playerContainerView.IsVisible)
        {
            return;
        }

        playerContainerView.ToggleContainer(playerInventoryContainer.Container);
    }

    public void ShowPlayerAndTempInventory(Container tempContainer)
    {
        if (playerContainerView.IsVisible || (tempContainerView && tempContainerView.IsVisible))
        {
            return;
        }

        playerContainerView.ToggleContainer(playerInventoryContainer.Container);

        tempInventoryContainer = tempContainer;

        CreateAndShowTempInventory();
    }

    public void HideInventories()
    {
        if (playerContainerView.IsVisible)
        {
            playerContainerView.ToggleContainer(playerInventoryContainer.Container);
        }

        if (tempContainerView && tempContainerView.IsVisible)
        {
            tempContainerView.ToggleContainer(tempInventoryContainer);
            inventorySystem.RemoveContainerGrid(InvPos.TemporaryInventory);
        }
    }

    private void CreateAndShowTempInventory()
    {
        tempContainerView =
            inventorySystem.CreateOrGetContainerGridInPosition(tempInventoryContainer, InvPos.TemporaryInventory);
        tempContainerView.ToggleContainer(tempInventoryContainer);
    }

    private void AssertDesignerFields()
    {
        Debug.Assert(inventorySystem,
            $"{nameof(inventorySystem)} is required for {nameof(InventoryViewController)}", this);
        Debug.Assert(playerInventoryContainer,
            $"{nameof(playerInventoryContainer)} is required for {nameof(InventoryViewController)}", this);
        Debug.Assert(menuPrefab,
            $"{nameof(menuPrefab)} is required for {nameof(InventoryViewController)}", this);
        Debug.Assert(graphicRaycaster,
            $"{nameof(graphicRaycaster)} is required for {nameof(InventoryViewController)}", this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }

        var clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (!clickedObject)
        {
            return;
        }

        var slotView = clickedObject.GetComponent<SlotView>();

        if (!slotView)
        {
            return;
        }

        var menuInstance = Instantiate(menuPrefab, playerContainerView.SlotContent.parent);

        menuInstance.transform.position = Input.mousePosition;
        menuInstance.Initialize(slotView.itemSlot);
    }
}