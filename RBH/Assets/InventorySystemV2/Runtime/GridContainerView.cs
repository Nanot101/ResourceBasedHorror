using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridContainerView : ContainerView
    {
        private GridLayoutGroup gridLayoutGroup;
        [ReadOnly] public Container container;

        private InventorySystem.InventoryPositionType inventoryPositionType;

        public InventorySystem.InventoryPositionType InventoryPositionType { get => inventoryPositionType; }

        public static Action<GridContainerView> OnInventoryOpen;
        public static Action<GridContainerView> OnInventoryClosed;


        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }

        public void ToggleContainer(Container _container)
        {
            if (IsVisible)
            {
                if (container != null)
                {
                    if (container != _container)
                    {
                        ShowContainer(_container);
                        return;
                    }
                }


                HideContainer();
            }
            else
                ShowContainer(_container);
        }

        public override void ShowContainer(Container _container)
        {
            base.ShowContainer(_container);
            container = _container;
            gridLayoutGroup.constraintCount = _container.containerWidth;
            Initialize(_container);
            OnInventoryOpen?.Invoke(this);
        }
        //Makes me able to create and initialize it entirely from code
        public void ShowContainer(Container _container, SlotView _slotViewPrefab)
        {
            slotViewPrefab = _slotViewPrefab;
            slotContent = transform;
            base.ShowContainer(_container);
            container = _container;
            gridLayoutGroup.constraintCount = _container.containerWidth;
            Initialize(_container);
        }
        //I think i can make it more clear but the inventory system should simplify setting up inventories
        //Essentially i'm automatizing what would be done manually
        public void Setup(Container _container, InventorySystem.InventoryPositionType _positionType)
        {
            container = _container;
            slotContent = transform;
            inventoryPositionType = _positionType;
        }

        private void Initialize(Container _container)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Initialize(_container.itemSlots[i], (int)gridLayoutGroup.cellSize.x, _container, this);
            }
        }
        public Vector2Int GetGridPos(int index)
        {
            int column = (index % gridLayoutGroup.constraintCount);
            int row = Mathf.FloorToInt(index / gridLayoutGroup.constraintCount);
            return new Vector2Int(column, row);
        }

        public SlotView GetSlotViewByPos(Vector2Int gridPos)
        {
            int index = gridPos.y * gridLayoutGroup.constraintCount + gridPos.x;
            return slots[index];
        }
        public SlotView GetSlotViewByIndex(int index)
        {
            return slots[index];
        }

        public void HighlightSlots(Vector2Int startGridPos, Vector2Int itemSize, bool canPlace)
        {
            int containerWidth = container.containerWidth;

            int containerHeight = container.itemSlots.Count / containerWidth;
            Color highlightColor = canPlace ? new Color(0, 1, 0, 0.1f) : new Color(1, 0, 0, 0.1f); //Red or green

            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    int gridX = startGridPos.x + x;
                    int gridY = startGridPos.y + y;

                    // Check bounds
                    if (gridX < 0 || gridX >= containerWidth || gridY < 0 || gridY >= containerHeight)
                        continue;

                    int index = container.GetIndexFromGridPos(gridX, gridY);
                    SlotView slotView = slots[index];

                    slotView.SetHighlight(highlightColor);
                }
            }
        }
        public void ClearHighlight()
        {
            foreach (var slotView in slots)
            {
                slotView.ClearHighlight();
            }
        }

        public override void HideContainer()
        {
            base.HideContainer();
            OnInventoryClosed?.Invoke(this);
        }

    }
}
