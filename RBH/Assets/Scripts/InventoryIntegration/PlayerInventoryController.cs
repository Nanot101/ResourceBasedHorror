using System;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField] private InventoryViewController viewController;

    private void Start()
    {
        Debug.Assert(viewController,
            $"{nameof(viewController)} is required for {nameof(PlayerInventoryController)}", this);
    }

    private void Update()
    {
        if (GamePause.IsPaused && !GamePause.IsPauseRequested<PlayerInventoryController>())
        {
            // The game is paused, but not by us.
            return;
        }

        if (!InputManager.Instance.OpenInventory)
        {
            return;
        }

        if (GamePause.IsPauseRequested<PlayerInventoryController>())
        {
            HideInventory();

            return;
        }

        ShowInventory();
    }

    private void ShowInventory()
    {
        viewController.ShowPlayerInventory();

        GamePause.RequestPause<PlayerInventoryController>();

        Time.timeScale = 0;
    }

    private void HideInventory()
    {
        viewController.HideInventories();

        GamePause.RequestResume<PlayerInventoryController>();

        Time.timeScale = 1;
    }
}