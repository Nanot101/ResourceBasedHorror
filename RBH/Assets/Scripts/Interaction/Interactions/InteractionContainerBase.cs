using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
[RequireComponent(typeof(ContainerHandler))]
public class InteractionContainerBase : InteractionBase
{
    private InventorySystem.InventorySystem inventorySystem;
    private InventoryViewController inventoryViewController;
    private InventoryOpenCloseAudio openCloseAudio;
    protected ContainerHandler containerHandler;

    private Coroutine interactionCooldownCoroutine;
    private bool isOpen = false;
    protected virtual void Start()
    {
        containerHandler = GetComponent<ContainerHandler>();
        inventorySystem = GameObject.FindGameObjectWithTag("InventorySystem").GetComponent<InventorySystem.InventorySystem>();
        inventoryViewController = inventorySystem.GetComponentInChildren<InventoryViewController>(); //Expecting only Player Inventory View
        openCloseAudio = inventorySystem.GetComponentInChildren<InventoryOpenCloseAudio>();
    }

    private void Update()
    {
        if (isOpen&& interactionCooldownCoroutine == null)
        {
            if (InputManager.Instance.Interact||InputManager.Instance.Cancel)
            {
                inventoryViewController.HideInventories();
                openCloseAudio.PlayClose();
                GamePause.RequestResume<InteractionContainerBase>();
                interactionCooldownCoroutine = StartCoroutine(InteractionCooldown());
            }
        }
    }
    private IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(1);
        isOpen = false;
        interactionCooldownCoroutine = null;
    }
    public override void Interact(IInteractionCaller caller)
    {
        if (isOpen)
            return;
        inventoryViewController.ShowPlayerAndTempInventory(containerHandler.Container);
        openCloseAudio.PlayOpen();
        GamePause.RequestPause<InteractionContainerBase>();
        isOpen = true;
    }


}
