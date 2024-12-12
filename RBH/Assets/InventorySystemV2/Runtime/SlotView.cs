using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem
{
    public class SlotView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private GridContainerView gridContainerView;
        private Container container;
        public ItemSlot itemSlot;
        public bool isEmpty;
        [SerializeField] private TextMeshProUGUI stackAmountText;
        [SerializeField] private Image itemImage;

        private int slotSize;

        private RectTransform slotRectTransform;
        private RectTransform imageRectTransform;

        private Vector2 originalRotation;

        private Vector3 originalImagePosition;
        SlotView rootSlotView;

        private Image highlightOverlay;
        bool isDragging;
        private void Update()
        {
            if (itemSlot == null)
            {
                return;
            }
            isEmpty = itemSlot.IsEmpty;

            if (Input.GetKeyDown(KeyCode.R) && isDragging)
            {
                if (itemSlot.GetItemStack() != null)
                {
                    itemSlot.GetItemStack().Rotate();
                    RotateImage(itemSlot.GetItemStack().IsRotated);
                    Debug.Log(itemSlot.GetItemStack().IsRotated);
                }
            }
        }
        private void OnDestroy()
        {
            Destroy(itemImage.gameObject);
            if (itemSlot != null)
            {
                itemSlot.OnItemChanged -= OnItemChanged;
                itemSlot.OnItemRemoved -= OnItemRemoved;
            }
        }
        private void OnDisable()
        {
            itemImage.gameObject.SetActive(false);
        }
        public void Initialize(ItemSlot _itemSlot, int _slotSize, Container _container, GridContainerView _gridContainerView)
        {
            gridContainerView = _gridContainerView;
            container = _container;
            slotSize = _slotSize;
            itemSlot = _itemSlot;
            itemSlot.OnItemChanged += OnItemChanged;
            itemSlot.OnItemRemoved += OnItemRemoved;
            //gridContainerView = _gridContainerView;
            //index = _index;
            slotRectTransform = GetComponent<RectTransform>();
            imageRectTransform = itemImage.GetComponent<RectTransform>();
            slotRectTransform.sizeDelta = new Vector2(slotSize, slotSize);
            imageRectTransform.sizeDelta = new Vector2(slotSize, slotSize);
            imageRectTransform.transform.SetParent(transform.parent);
            SetPivotAndAnchor(slotRectTransform, imageRectTransform);
            imageRectTransform.pivot = new Vector2(0.5f, 0.5f);

            UpdateView();
        }

        private void OnItemChanged(ItemSlot slot)
        {
            UpdateView();
        }

        private void OnItemRemoved(ItemSlot slot)
        {
            UpdateView();
        }

        [Button]
        private void RemoveItemInSlot()
        {
            if (!itemSlot.HasItemStack)
            {
                return;
            }
            ResetImage();
            container.RemoveItem(itemSlot.rootIndex);
        }

        [Button]
        private void UpdateView()
        {
            if (itemSlot == null || itemSlot.IsEmpty)
            {
                itemImage.gameObject.SetActive(false);
                stackAmountText.gameObject.SetActive(false);
                return;
            }
            ItemStack itemStack = itemSlot.GetItemStack();
            if (itemSlot.ItemPositionInGrid == Vector2Int.zero)
            {
                itemImage.sprite = itemStack.ItemData.ItemSprite;

                ResizeItemImage(slotRectTransform, imageRectTransform);
                RotateImage(itemStack.IsRotated);
                SetPositionItemImage(slotRectTransform, imageRectTransform);

                itemImage.gameObject.SetActive(true);

                //itemSlot.ItemData.Icon

            }
            else
            {
                itemImage.gameObject.SetActive(false);
                stackAmountText.gameObject.SetActive(false);
            }
            if (itemSlot.ItemData.IsStackable)
            {
                stackAmountText.gameObject.SetActive(true);
                stackAmountText.text = itemSlot.GetItemStack().Amount.ToString();
            }
            else
            {
                stackAmountText.gameObject.SetActive(false);
            }
        }

        private void ResizeItemImage(RectTransform slotRectTransform, RectTransform imageRectTransform)
        {
            Vector2 slotSize = slotRectTransform.rect.size;
            Vector2 itemSize;
            itemSize = new Vector2(slotSize.x * itemSlot.ItemData.Size.x, slotSize.y * itemSlot.ItemData.Size.y);


            imageRectTransform.sizeDelta = itemSize;
        }

        void SetPositionItemImage(RectTransform slotRectTransform, RectTransform imageRectTransform)
        {
            //Vector3 worldPosition = slotRectTransform.TransformPoint(Vector3.zero);

            //RectTransform containerTransform = itemImage.transform.parent.GetComponent<RectTransform>();
            //Vector3 localPosition = containerTransform.InverseTransformPoint(worldPosition);

            //imageRectTransform.localPosition = localPosition + new Vector3(slotRectTransform.rect.size.x * itemSlot.ContainerPosition.x, -slotRectTransform.rect.size.y * itemSlot.ContainerPosition.y, 0);
            Vector2 itemPosition = new Vector2(slotSize * itemSlot.ContainerPosition.x, -slotSize * itemSlot.ContainerPosition.y);
            //if (itemSlot.GetItemStack().IsRotated)
            //{
            //    // Adjust position for rotated items
            //    itemPosition += new Vector2(slotSize * itemSlot.ItemSize.y, 0);
            //}
            // Set the anchored position of the image relative to the container
            imageRectTransform.anchoredPosition = itemPosition;

            if (itemSlot.GetItemStack().IsRotated)
                imageRectTransform.anchoredPosition -= new Vector2(0, itemSlot.ItemData.Size.x * slotSize);
        }

        void SetPivotAndAnchor(RectTransform slotRectTransform, RectTransform imageRectTransform)
        {
            Vector2 pivotAndAnchor = new Vector2(0, 1);

            slotRectTransform.pivot = pivotAndAnchor;
            slotRectTransform.anchorMin = pivotAndAnchor;
            slotRectTransform.anchorMax = pivotAndAnchor;

            imageRectTransform.anchorMin = pivotAndAnchor;
            imageRectTransform.anchorMax = pivotAndAnchor;
        }
        void ResetImagePivotAndAnchor(RectTransform imageRectTransform)
        {
            Vector2 pivotAndAnchor = new Vector2(0.5f, 0.5f);
            imageRectTransform.pivot = pivotAndAnchor;
            imageRectTransform.anchorMin = pivotAndAnchor;
            imageRectTransform.anchorMax = pivotAndAnchor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                RemoveItemInSlot();
            }
        }

        private void ResetImage()
        {
            imageRectTransform.sizeDelta = new Vector2(slotSize, slotSize);
            imageRectTransform.rect.Set(0, 0, slotSize, slotSize);
        }

        private void UpdatePreviewHighlight(Vector2Int gridPos)
        {
            gridContainerView.ClearHighlight();

            Vector2Int itemSize = itemSlot.ItemSize;

            bool canPlace = container.CanPlaceItemAt(gridPos.x, gridPos.y, itemSize, itemSlot);

            gridContainerView.HighlightSlots(gridPos, itemSize, canPlace);
        }


        public void SetHighlight(Color highlightColor)
        {
            if (highlightOverlay == null)
            {
                GameObject highlightObj = new GameObject("HighlightOverlay");
                highlightObj.transform.SetParent(transform);
                highlightOverlay = highlightObj.AddComponent<Image>();
                highlightOverlay.raycastTarget = false;
                //highlightObj.GetComponent<RectTransform>().sizeDelta = new Vector2(slotSize, slotSize);
                highlightOverlay.rectTransform.anchorMin = Vector2.zero;
                highlightOverlay.rectTransform.anchorMax = Vector2.one;
                highlightOverlay.rectTransform.offsetMin = Vector2.zero;
                highlightOverlay.rectTransform.offsetMax = Vector2.zero;
                highlightOverlay.rectTransform.localScale = Vector3.one;
                highlightOverlay.color = highlightColor;
            }
            highlightOverlay.color = highlightColor;
            highlightOverlay.gameObject.SetActive(true);
        }
        public void ClearHighlight()
        {
            if (highlightOverlay != null)
            {
                highlightOverlay.gameObject.SetActive(false);
            }
        }
        public void ForceBaseImageRotation(bool value)
        {
            ResetImage();
            //imageRectTransform.SetParent(transform);
            ResetImagePivotAndAnchor(imageRectTransform);
            if (value)
            {
                RotateImage(true);
            }
            else
            {
                RotateImage(false);
            }
            //imageRectTransform.transform.SetParent(transform.parent);
            SetPivotAndAnchor(slotRectTransform, imageRectTransform);

        }
        public void RotateImage(bool isRotated)
        {
            if (isRotated)
            {
                imageRectTransform.pivot = new Vector2(0f, 1f);
                imageRectTransform.localEulerAngles = new Vector3(0, 0, 90);


                // Swap width and height
                //Vector2 originalSize = imageRectTransform.sizeDelta;
                //imageRectTransform.sizeDelta = new Vector2(originalSize.y, originalSize.x);

                // Adjust pivot to maintain position
                //imageRectTransform.   pivot = new Vector2(0, 1f);
                //imageRectTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                imageRectTransform.localEulerAngles = Vector3.zero;

                // Swap width and height back
                //Vector2 originalSize = imageRectTransform.sizeDelta;
                //imageRectTransform.sizeDelta = new Vector2(originalSize.x, originalSize.y);

                // Reset pivot to original
                imageRectTransform.pivot = new Vector2(0, 1);
            }
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItemStack)
            {
                return;
            }
            if (itemSlot.ItemPositionInGrid != Vector2Int.zero)
            {
                // Get the root slot view based on the root index
                rootSlotView = gridContainerView.GetSlotViewByIndex(itemSlot.rootIndex);
            }
            else
            {
                rootSlotView = this;
            }
            rootSlotView.imageRectTransform.SetAsLastSibling();

            originalImagePosition = rootSlotView.imageRectTransform.localPosition;
            isDragging = true;

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItemStack)
            {
                return;
            }
            rootSlotView.imageRectTransform.position = Input.mousePosition;

            GameObject targetUnderCursor = eventData.pointerCurrentRaycast.gameObject;

            if (targetUnderCursor == null)
                return;
            SlotView targetSlot = targetUnderCursor.GetComponent<SlotView>();

            if (targetSlot == null)
                return;



            UpdatePreviewHighlight(targetSlot.itemSlot.ContainerPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (itemSlot.IsEmpty)
            {
                return;
            }
            GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;
            if (dropTarget == null)
            {
                ReturnSlotState();
                Debug.Log("Drop target is null");
                return;
            }
            SlotView dropTargetSlot = dropTarget.GetComponent<SlotView>();
            if (dropTargetSlot == null)
            {
                ReturnSlotState();
                Debug.Log("Drop target slot is null");
                return;
            }
            else
            {
                if (dropTargetSlot.itemSlot.Index == itemSlot.rootIndex)
                {
                    Debug.Log("Dropped on the same slot");
                    ReturnSlotState();
                    return;
                }
            }
            ItemStack itemStack = itemSlot.GetItemStack();
            container.RemoveItem(itemSlot.rootIndex);
            if (dropTargetSlot.itemSlot.HasItemStack)
            {
                //for now return later make a way to swap if there is 1 item in the required slots
                container.AddItemAt(itemStack, rootSlotView.itemSlot.ContainerPosition.x, rootSlotView.itemSlot.ContainerPosition.y);
                ReturnSlotState();
                Debug.Log("There is already an item here and i can't swap yet");
                return;
            }
            else
            {
                //I could try moving the item to the new slot but right now i'll just remove and add it there
                Vector2Int dropSlotPos = dropTargetSlot.itemSlot.ContainerPosition;
                bool success = container.AddItemAt(itemStack, dropSlotPos.x, dropSlotPos.y);
                if (success)
                {
                    gridContainerView.ClearHighlight();
                    Debug.Log("Sucessfully added");
                    Debug.Log(itemStack.IsRotated);
                }
                else
                {
                    container.AddItemAt(itemStack, rootSlotView.itemSlot.ContainerPosition.x, rootSlotView.itemSlot.ContainerPosition.y);
                    ReturnSlotState();
                    Debug.LogWarning("Failed to add");
                }
            }
            isDragging = false;
        }

        private void ReturnSlotState()
        {
            gridContainerView.ClearHighlight();
            rootSlotView.imageRectTransform.localPosition = originalImagePosition;
        }

    }
}
