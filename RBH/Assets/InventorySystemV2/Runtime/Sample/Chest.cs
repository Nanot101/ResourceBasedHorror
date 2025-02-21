using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(ContainerHandler))]
    public class Chest : MonoBehaviour
    {
        public ContainerHandler containerHandler;
        public GridContainerView gridContainerView;
        public InventorySystem.InventoryPositionType positionType = InventorySystem.InventoryPositionType.ChestInventory;

        public KeyCode openChestKeycode;

        [SerializeField] private InventorySystem system;

        private void Awake()
        {
            containerHandler = GetComponent<ContainerHandler>();

        }

        private void Start()
        {
            containerHandler = GetComponent<ContainerHandler>();
            //gridContainerView = system.CreateOrGetContainerGridInPosition(containerHandler.Container,InventorySystem.InventoryPositionType.ChestInventory);
            gridContainerView = system.CreateOrGetContainerGridInPosition(containerHandler.Container,positionType);
        }

        private void Update()
        {
            //Use player interaction
            if (InputManager.Instance.Interact)
            {
                gridContainerView.ToggleContainer(containerHandler.Container);
            }
        }

    }
}
