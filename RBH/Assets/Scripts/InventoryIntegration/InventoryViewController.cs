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
public class InventoryViewController : MonoBehaviour
{
    [SerializeField] private InvSys inventorySystem;
    [SerializeField] private ContainerHandler playerInventoryContainer;
    [SerializeField] private CtxMenu menuPrefab;

    [SerializeField] private GraphicRaycaster graphicRaycaster;

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
}