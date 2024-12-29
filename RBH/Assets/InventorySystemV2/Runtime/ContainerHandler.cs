using Sirenix.OdinInspector;
using UnityEngine;
namespace InventorySystem
{
    public class ContainerHandler : MonoBehaviour
    {
        //Used for identification, container names can be: Player Chest MonsterBody etc
        [SerializeField] private string containerName;
        [SerializeField] private int initialSlotAmount;
        public ItemStack[] initialItems;
        [SerializeField] private int containerWidth;

        [ShowInInspector] private Container container;
        public Container Container { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (IsInitialized) return;
            Container = new Container(containerName,initialSlotAmount, containerWidth, initialItems);
            container = Container;
            IsInitialized = true;
        }
    }
}
