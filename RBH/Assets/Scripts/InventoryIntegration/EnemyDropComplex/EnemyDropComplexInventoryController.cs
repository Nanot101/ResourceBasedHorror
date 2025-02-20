using InventorySystem;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDropComplexInventoryController : Singleton<EnemyDropComplexInventoryController>
{
    [SerializeField] private InventoryViewController viewController;

    [SerializeField] private InventoryContainerDropItemSynchronization dropItemSynchronization;

    [SerializeField] private InventoryOpenCloseAudio openCloseAudio;

    [SerializeField] private int tempContainerSlots = 32;
    [SerializeField] private int tempContainerWidth = 4;

    private Container temporaryInventoryContainer;

    private void Start()
    {
        Debug.Assert(viewController,
            $"{nameof(viewController)} is required for {nameof(EnemyDropComplexInventoryController)}", this);
        Debug.Assert(openCloseAudio,
            $"{nameof(openCloseAudio)} is required for {nameof(EnemyDropComplexInventoryController)}", this);
    }

    private void Update()
    {
        if (GamePause.IsPaused && !GamePause.IsPauseRequested<EnemyDropComplexInventoryController>())
        {
            return;
        }

        if (!InputManager.Instance.OpenInventory)
        {
            return;
        }

        if (!GamePause.IsPauseRequested<EnemyDropComplexInventoryController>())
        {
            ManualTriggerDropComplexInventory();
            return;
        }

        HideInventory();

        ClearTemporaryContainer();
    }

    public void TriggerDropComplexInventory(Transform playerTransform)
    {
        temporaryInventoryContainer = new Container("Temporary Container", tempContainerSlots, tempContainerWidth);

        dropItemSynchronization.StartSynchronization(playerTransform.position, temporaryInventoryContainer);

        ShowInventory();
    }

    private void ManualTriggerDropComplexInventory()
    {
        var playerInteractionCaller = FindObjectOfType<PlayerInteractionCaller>();

        if (!playerInteractionCaller)
        {
            Debug.LogError("PlayerInteractionCaller not found in scene.", this);
            return;
        }

        TriggerDropComplexInventory(playerInteractionCaller.gameObject.transform);
    }

    private void ClearTemporaryContainer()
    {
        dropItemSynchronization.StopSynchronization();

        temporaryInventoryContainer.Clear();
        temporaryInventoryContainer = null;
    }

    private void ShowInventory()
    {
        viewController.ShowPlayerAndTempInventory(temporaryInventoryContainer);

        openCloseAudio.PlayOpen();
        
        GamePause.RequestPause<EnemyDropComplexInventoryController>();
    }

    private void HideInventory()
    {
        viewController.HideInventories();

        GamePause.RequestResume<EnemyDropComplexInventoryController>();
        
        openCloseAudio.PlayClose();
    }
}