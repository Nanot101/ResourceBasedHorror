using InventorySystem;
using UnityEngine;

public class EnemyDropComplexInventoryController : Singleton<EnemyDropComplexInventoryController>
{
    [SerializeField] private InventoryViewController viewController;

    [SerializeField] private int tempContainerSlots = 32;
    [SerializeField] private int tempContainerWidth = 4;

    private Container tempContainer;

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

        DropItemsInTempContainer();
    }

    public void StartDropComplexInventory(Transform playerTransform)
    {
        PickUpItemsAroundPlayer();

        ShowInventory();
    }

    private void PickUpItemsAroundPlayer()
    {
    }

    private void DropItemsInTempContainer()
    {
        tempContainer = null;
    }

    private void ShowInventory()
    {
        viewController.ShowPlayerAndTempInventory(tempContainer);

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