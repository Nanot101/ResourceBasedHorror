using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    [RequireComponent(typeof(ItemDatabaseHandler))]
    public class InventorySystem : SerializedMonoBehaviour
    {
        private List<GridContainerView> inventories = new List<GridContainerView>();
        public Transform inventoryHolder;

        public enum InventoryPositionType
        {
            PlayerInventory,
            TemporaryInventory,
            ChestInventory
        }
        public Dictionary<InventoryPositionType, Transform> predefinedPositions = new()
    {
        { InventoryPositionType.PlayerInventory, null },
        { InventoryPositionType.TemporaryInventory, null },
        { InventoryPositionType.ChestInventory, null }
    };
        [SerializeField] private GridContainerView inventoryGridPrefab;
        [SerializeField] private SlotView slotViewPrefab;

        private GridContainerView CreateContainer(Container targetContainer, InventoryPositionType positionType)
        {
            GridContainerView containerView = Instantiate(inventoryGridPrefab, inventoryHolder.transform);
            containerView.gameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            containerView.transform.position = GetPositionFromPositionType(positionType).position;
            containerView.gameObject.name = $"{positionType}";
            containerView.Setup(targetContainer,positionType);
            inventories.Add(containerView);
            return containerView;
        }
        public GridContainerView CreateOrGetContainerGridInPosition(Container targetContainer, InventoryPositionType positionType)
        {
            GridContainerView currentGCV = GetGridContainerView(positionType);
            if (currentGCV != null)
            {
                return currentGCV;
            }
            return CreateContainer(targetContainer, positionType);
        }

        public void RemoveContainerGrid(InventoryPositionType position)
        {
            GridContainerView containerView = GetGridContainerView(position);
            if (containerView == null)
            {
                Debug.LogWarning($"Didn't find any container views in {position}. Removal failed");
                return;
            }
            inventories.Remove(containerView);
            Destroy(containerView);
        }

        public GridContainerView GetGridContainerView(Container targetContainer)
        {
            GridContainerView targetGrid = inventories.FirstOrDefault(inv => inv.container == targetContainer);
            return targetGrid;
        }

        public GridContainerView GetGridContainerView(InventoryPositionType type)
        {
            GridContainerView targetGrid = inventories.FirstOrDefault(inv => inv.InventoryPositionType == type);
            return targetGrid;
        }

        private Transform GetPositionFromPositionType(InventoryPositionType type)
        {
            Transform position = predefinedPositions.GetValueOrDefault(type);
            if (position == null) {
                Debug.LogError("Position not assigned or transform not found in inventory system's predefined positions");
                return transform;
            }
            return position;
        }
    }
}
