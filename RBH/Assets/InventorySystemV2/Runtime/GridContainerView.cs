using Sirenix.OdinInspector;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
namespace InventorySystem
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridContainerView : ContainerView
    {
        private GridLayoutGroup gridLayoutGroup;
        private Container container;

        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
        }
        public override void ShowContainer(Container _container)
        {
            base.ShowContainer(_container);
            container = _container;
            gridLayoutGroup.constraintCount = _container.containerWidth;
            Initialize(_container);
        }

        private void Initialize(Container _container)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Initialize(_container.itemSlots[i],(int)gridLayoutGroup.cellSize.x,_container,this);
            }
        }
        public Vector2Int GetGridPos(int index)
        {
            int column = (index % gridLayoutGroup.constraintCount);
            int row = Mathf.FloorToInt(index / gridLayoutGroup.constraintCount);
            return new Vector2Int(column,row);
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

    }
}
