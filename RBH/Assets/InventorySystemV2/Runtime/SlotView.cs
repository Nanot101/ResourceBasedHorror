using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem
{
    public class SlotView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

        private SlotView currentTargetSlot;
        private void Update()
        {
            if (itemSlot == null)
            {
                return;
            }
            isEmpty = itemSlot.IsEmpty;
            if (currentTargetSlot == null)
                return;
            if (Input.GetKeyDown(KeyCode.R) && isDragging)
            {
                wantsRotate = !wantsRotate;
                if (tempPreviewImage == null)
                    return;
                bool rotate = wantsRotate ? !itemSlot.GetItemStack().IsRotated : itemSlot.GetItemStack().IsRotated;
                RotateTempImage(rotate);
                UpdateCurrentPreviewHighlight(currentTargetSlot);
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
            if (tempPreviewImage)
            {
                Destroy(tempPreviewImage);
                if (currentTargetSlot != null)
                {
                    currentTargetSlot.gridContainerView.ClearHighlight();
                }
            }
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
                RotateImage(imageRectTransform, itemStack.IsRotated);
                SetPositionItemImage(imageRectTransform, itemStack.IsRotated);

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

        void SetPositionItemImage(RectTransform imageRectTransform, bool isRotated)
        {

            Vector2 itemPosition = new Vector2(slotSize * itemSlot.ContainerPosition.x, -slotSize * itemSlot.ContainerPosition.y);

            // Set the anchored position of the image relative to the container
            imageRectTransform.anchoredPosition = itemPosition;

            if (isRotated)
                imageRectTransform.anchoredPosition -= new Vector2(0, itemSlot.ItemData.Size.x * slotSize);
        }

        void SetPivotAndAnchor(RectTransform slotRectTransform, RectTransform imageRectTransform)
        {
            Vector2 pivotAndAnchor = new Vector2(0, 1);

            slotRectTransform.pivot = pivotAndAnchor;
            slotRectTransform.anchorMin = pivotAndAnchor;
            slotRectTransform.anchorMax = pivotAndAnchor;

            SetPivot(imageRectTransform, pivotAndAnchor);
        }

        private static void SetPivot(RectTransform imageRectTransform, Vector2 pivotAndAnchor)
        {
            imageRectTransform.pivot = pivotAndAnchor;
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



        private void ResetImage()
        {
            imageRectTransform.sizeDelta = new Vector2(slotSize, slotSize);
            imageRectTransform.rect.Set(0, 0, slotSize, slotSize);
        }

        private void UpdatePreviewHighlight(Vector2Int gridPos, Vector2Int itemSize, Container targetContainer, GridContainerView targetContainerView)
        {
            targetContainerView.ClearHighlight();

            bool canPlace = targetContainer.CanPlaceItemAt(gridPos.x, gridPos.y, itemSize, itemSlot);

            targetContainerView.HighlightSlots(gridPos, itemSize, canPlace);
        }
        private void UpdateCurrentPreviewHighlight(SlotView targetSlot)
        {
            if (wantsRotate)
            {
                Vector2Int itemSize;
                itemSize = itemSlot.GetItemStack().IsRotated ? itemSlot.ItemData.Size : InvertedSize(itemSlot.ItemData.Size);
                UpdatePreviewHighlight(targetSlot.itemSlot.ContainerPosition, itemSize, targetSlot.container, targetSlot.gridContainerView);
            }
            else
            {
                UpdatePreviewHighlight(targetSlot.itemSlot.ContainerPosition, itemSlot.ItemSize, targetSlot.container, targetSlot.gridContainerView);
            }
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

        #region Rotation

        private Image tempPreviewImage;
        bool wantsRotate;


        public void RotateImage(RectTransform imageRectTransform, bool isRotated)
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

        public void RotateTempImage(bool isRotated)
        {
            if (isRotated)
            {
                tempPreviewImage.rectTransform.localEulerAngles = new Vector3(0, 0, 90);
            }
            else
            {
                tempPreviewImage.rectTransform.localEulerAngles = Vector3.zero;
            }
        }

        #endregion

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItemStack)
            {
                return;
            }
            if (itemSlot.ItemPositionInGrid != Vector2Int.zero)
            {
                //I'm not root
                // Get the root slot view reference based on the root index
                rootSlotView = gridContainerView.GetSlotViewByIndex(itemSlot.rootIndex);
            }
            else
            {
                //I'm root
                rootSlotView = this;
            }
            //Create Temporary Image
            GameObject imagePreview = new GameObject();
            imagePreview.SetActive(false);
            imagePreview.AddComponent<LayoutElement>().ignoreLayout = true;
            tempPreviewImage = imagePreview.AddComponent<Image>();
            tempPreviewImage.raycastTarget = false;
            tempPreviewImage.name = "TempPreviewImage";
            tempPreviewImage.sprite = itemSlot.ItemData.ItemSprite;
            tempPreviewImage.rectTransform.sizeDelta = rootSlotView.imageRectTransform.sizeDelta;
            //SetPivot(tempPreviewImage.rectTransform, new Vector2(0, 1));
            tempPreviewImage.rectTransform.SetParent(transform.parent.parent);
            tempPreviewImage.rectTransform.SetAsLastSibling();
            tempPreviewImage.rectTransform.localScale = Vector2.one;

            //RotateImage(tempPreviewImage.rectTransform,itemSlot.GetItemStack().IsRotated);
            RotateTempImage(itemSlot.GetItemStack().IsRotated);
            //SetPositionItemImage(tempPreviewImage.rectTransform, itemSlot.GetItemStack().IsRotated);
            rootSlotView.imageRectTransform.gameObject.SetActive(false);
            rootSlotView.imageRectTransform.SetAsLastSibling();
            originalImagePosition = rootSlotView.imageRectTransform.localPosition;
            imagePreview.SetActive(true);
            isDragging = true;

        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!itemSlot.HasItemStack)
            {
                return;
            }
            //offset to look better when moving item around
            Vector3 offset = new(10, -10, 0);

            tempPreviewImage.rectTransform.position = Input.mousePosition + offset;

            GameObject targetUnderCursor = eventData.pointerCurrentRaycast.gameObject;

            if (currentTargetSlot != null)
            {
                if (currentTargetSlot.container != container)
                {
                    gridContainerView.ClearHighlight();
                    currentTargetSlot.gridContainerView.ClearHighlight();
                }
            }
            if (targetUnderCursor == null)
            {
                gridContainerView.ClearHighlight();
                return;
            }
            SlotView targetSlot = targetUnderCursor.GetComponent<SlotView>();
            currentTargetSlot = targetSlot;

            if (targetSlot == null)
            {
                gridContainerView.ClearHighlight();
                return;
            }
            UpdateCurrentPreviewHighlight(targetSlot);
        }



        public void OnEndDrag(PointerEventData eventData)
        {
            if (itemSlot.IsEmpty)
            {
                return;
            }
            Destroy(tempPreviewImage.gameObject);
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
            if (dropTargetSlot.container == container)
            {
                if (dropTargetSlot.itemSlot.Index == itemSlot.rootIndex && !wantsRotate)
                {
                    Debug.Log("Dropped on the same slot");
                    ReturnSlotState();
                    return;
                }
            }

            if (!dropTargetSlot.isEmpty)
            {
                ReturnSlotState();
                return;
            }
            ItemStack itemStack = itemSlot.GetItemStack();
            container.RemoveItem(itemSlot.rootIndex);
            if (dropTargetSlot.itemSlot.HasItemStack)
            {
                //for now return later make a way to swap if there is 1 item in the required slots
                if (dropTargetSlot.container != container)
                {
                    dropTargetSlot.container.AddItemAt(itemStack, rootSlotView.itemSlot.ContainerPosition.x, rootSlotView.itemSlot.ContainerPosition.y);
                }
                else
                {
                    container.AddItemAt(itemStack, rootSlotView.itemSlot.ContainerPosition.x, rootSlotView.itemSlot.ContainerPosition.y);
                }
                ReturnSlotState();
                Debug.Log("There is already an item here and i can't swap yet");
                return;
            }
            else
            {
                //I could try moving the item to the new slot but right now i'll just remove and add it there
                Vector2Int dropSlotPos = dropTargetSlot.itemSlot.ContainerPosition;
                if (wantsRotate)
                {
                    itemStack.Rotate();
                }
                bool success;
                if (dropTargetSlot.container == container)
                {
                    success = container.AddItemAt(itemStack, dropSlotPos.x, dropSlotPos.y);
                }
                else
                {
                    success = dropTargetSlot.container.AddItemAt(itemStack, dropSlotPos.x, dropSlotPos.y);
                }
                if (success)
                {
                    wantsRotate = false;
                    if (currentTargetSlot != null)
                    {
                        currentTargetSlot.gridContainerView.ClearHighlight();
                    }
                    else
                    {
                        gridContainerView.ClearHighlight();
                    }
                    Debug.Log("Sucessfully added");
                    Debug.Log(itemStack.IsRotated);
                }
                else
                {
                    if (wantsRotate)
                    {
                        itemStack.Rotate();
                    }
                    container.AddItemAt(itemStack, rootSlotView.itemSlot.ContainerPosition.x, rootSlotView.itemSlot.ContainerPosition.y);
                    ReturnSlotState();
                    Debug.LogWarning("Failed to add");
                }
            }
            isDragging = false;
        }

        private void ReturnSlotState()
        {
            wantsRotate = false;
            gridContainerView.ClearHighlight();
            if (currentTargetSlot != null)
            {
                currentTargetSlot.gridContainerView.ClearHighlight();
            }
            rootSlotView.imageRectTransform.gameObject.SetActive(true);
            rootSlotView.imageRectTransform.localPosition = originalImagePosition;
        }

        private Vector2Int InvertedSize(Vector2Int size)
        {
            return new Vector2Int(size.y, size.x);
        }

    }
}
