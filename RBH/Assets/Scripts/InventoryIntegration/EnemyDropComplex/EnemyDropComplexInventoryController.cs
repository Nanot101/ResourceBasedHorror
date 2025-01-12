using InventorySystem;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDropComplexInventoryController : Singleton<EnemyDropComplexInventoryController>
{
    [SerializeField] private InventoryViewController viewController;

    [FormerlySerializedAs("interactionCaller")] [SerializeField]
    private InventoryContainerDropItemSynchronization dropItemSynchronization;

    [SerializeField] private int tempContainerSlots = 32;
    [SerializeField] private int tempContainerWidth = 4;

    private Container temporaryInventoryContainer;
    private Transform playerTransform;

    private void Start()
    {
        Debug.Assert(viewController,
            $"{nameof(viewController)} is required for {nameof(EnemyDropComplexInventoryController)}", this);
    }

    private void Update()
    {
        if (GamePause.IsPaused && !GamePause.IsPauseRequested<EnemyDropComplexInventoryController>())
        {
            // The game is paused, but not by us.
            return;
        }

        if (!Input.GetKeyDown(viewController.KeyToOpenInventory) ||
            !GamePause.IsPauseRequested<EnemyDropComplexInventoryController>())
        {
            return;
        }

        HideInventory();

        ClearTemporaryContainer();
    }

    public void TriggerDropComplexInventory(Transform playerTransform)
    {
        this.playerTransform = playerTransform;

        temporaryInventoryContainer = new Container("Temporary Container", tempContainerSlots, tempContainerWidth);

        dropItemSynchronization.StartSynchronization(this.playerTransform.position, temporaryInventoryContainer);

        ShowInventory();
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

        GamePause.RequestPause<EnemyDropComplexInventoryController>();

        Time.timeScale = 0;
    }

    private void HideInventory()
    {
        viewController.HideInventories();

        GamePause.RequestResume<EnemyDropComplexInventoryController>();

        Time.timeScale = 1;
    }
}