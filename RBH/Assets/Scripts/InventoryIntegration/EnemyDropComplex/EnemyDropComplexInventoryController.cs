using System.Linq;
using InventorySystem;
using UnityEngine;

public class EnemyDropComplexInventoryController : Singleton<EnemyDropComplexInventoryController>
{
    [SerializeField] private InventoryViewController viewController;
    [SerializeField] private EnemyDropComplexInteractionCaller interactionCaller;
    [SerializeField] private InventoryDropSystem dropSystem;

    [SerializeField] private int tempContainerSlots = 32;
    [SerializeField] private int tempContainerWidth = 4;

    private Container temporaryInventoryContainer;
    private bool itemsPickUpStarted = false;
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

        DropItemsInTempContainer();
    }

    public void StartDropComplexInventory(Transform playerTransform)
    {
        if (itemsPickUpStarted)
        {
            Debug.LogWarning(
                "You can't start another complex item pickup until the previous one is finished. " +
                "This problem can occur when the temporary inventory of items from the ground is not " +
                "able to hold all the items from the ground. " +
                "This results in another start of a complex item pickup.",
                this);
            return;
        }

        itemsPickUpStarted = true;

        this.playerTransform = playerTransform;

        temporaryInventoryContainer = new Container("Temporary Container", tempContainerSlots, tempContainerWidth);

        interactionCaller.PickUpItemsAroundPlayer(this.playerTransform.position, temporaryInventoryContainer);

        itemsPickUpStarted = false;

        ShowInventory();
    }

    private void DropItemsInTempContainer()
    {
        var itemsToDropGroupedByItemData = temporaryInventoryContainer.itemSlots
            .Where(x => x.HasItemStack)
            .Select(x => x.GetItemStack())
            .OrderBy(x => x.Amount)
            .GroupBy(x => x.ItemData);

        foreach (var itemDataGroup in itemsToDropGroupedByItemData)
        {
            dropSystem.DropItem(itemDataGroup.Key, itemDataGroup.ToList(), playerTransform.position);
        }

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