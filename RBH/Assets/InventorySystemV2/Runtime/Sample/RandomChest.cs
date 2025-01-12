using UnityEngine;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    [RequireComponent(typeof(ContainerHandler))]
    public class RandomChest : MonoBehaviour, IPointerClickHandler
    {

        public ContainerHandler containerHandler;
        private GridContainerView gridContainerView;

        private InventorySystem system;

        void Start()
        {
            containerHandler = GetComponent<ContainerHandler>();
            system = GameObject.FindGameObjectWithTag("InventorySystem").GetComponent<InventorySystem>();
            gridContainerView = system.CreateOrGetContainerGridInPosition(containerHandler.Container, InventorySystem.InventoryPositionType.TemporaryInventory);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (gridContainerView.container == containerHandler.Container&&gridContainerView.IsVisible)
            {
                return;
            }
            containerHandler.Container.Clear();
            for (int i = 0; i < 6; i++)
            {
                int randomItemIndex = Random.Range(0,ItemDatabaseHandler.Instance.database.items.Count);
                containerHandler.Container.AddItem(new ItemStack(ItemDatabaseHandler.Instance.database.items[randomItemIndex]), out _);
            }
            gridContainerView.ShowContainer(containerHandler.Container);
        }
    }
}
