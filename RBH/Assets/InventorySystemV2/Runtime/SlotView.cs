using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace InventorySystem
{
    public class SlotView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        public SlotView rootSlotView;

        private Image highlightOverlay;
        bool isDragging;

        private SlotView currentTargetSlot;

        private Transform imageHolderTransform;


        #region Properties
        public GridContainerView GridContainerView { get { return gridContainerView; } }
        #endregion

        private void Update()
        {
            if (itemSlot == null)
            {
                isEmpty = true;
                return;
            }
            isEmpty = itemSlot.IsEmpty;
            if (currentTargetSlot == null)
                return;
            if (Input.GetKeyDown(KeyCode.R) && rootSlotView.isDragging)
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
            if (itemImage)
                Destroy(itemImage.gameObject);
            if (itemHighlightImageInstance != null)
                Destroy(itemHighlightImageInstance);
            if (itemSlot != null)
            {
                itemSlot.OnItemChanged -= OnItemChanged;
                itemSlot.OnItemRemoved -= OnItemRemoved;
            }
        }
        private void OnDisable()
        {
            if (itemImage)
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
            Transform tempImageHolder = GetImageHolderInParent();
            if (tempImageHolder == null)
            {
                GameObject imageHolder = new GameObject();
                imageHolder.name = "TempImageHolder";
                imageHolder.AddComponent<LayoutElement>().ignoreLayout = true;
                imageHolder.transform.SetParent(transform.parent);
                RectTransform imageHolderRectTransform = imageHolder.GetComponent<RectTransform>();
                imageHolderRectTransform.sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
                imageHolderRectTransform.pivot = new Vector2(0, 1);
                imageHolder.transform.localScale = Vector3.one;
                imageHolder.transform.position = transform.parent.position;
                tempImageHolder = imageHolder.transform;
                imageHolderTransform = imageHolder.transform;
            }
            imageHolderTransform = tempImageHolder.transform;
            imageRectTransform.transform.SetParent(imageHolderTransform);
            SetPivotAndAnchor(slotRectTransform, imageRectTransform);
            imageRectTransform.pivot = new Vector2(0.5f, 0.5f);

            UpdateView();
            if (tempImageHolder != null)
            {

                tempImageHolder.transform.SetAsLastSibling();
            }
        }

        private void OnItemChanged(ItemSlot slot)
        {
            UpdateView();
        }
        private Transform GetImageHolderInParent()
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i).name == "TempImageHolder")
                {
                    return transform.parent.GetChild(i).transform;
                }
            }
            return null;
        }
        private void OnItemRemoved(ItemSlot slot)
        {
            UpdateView();
        }


        private void RemoveItemInSlot()
        {
            if (!itemSlot.HasItemStack)
            {
                return;
            }
            ResetImage();
            container.RemoveItem(itemSlot.rootIndex);
        }


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
                //imageRectTransform.transform.SetParent(imageHolderTransform);

                itemImage.gameObject.SetActive(true);

                if (itemSlot.ItemData.IsStackable)
                {
                    stackAmountText.gameObject.SetActive(true);
                }
                else
                {
                    stackAmountText.gameObject.SetActive(false);
                }

                //itemSlot.ItemData.Icon
                rootSlotView = this;

            }
            else
            {
                itemImage.gameObject.SetActive(false);
                stackAmountText.gameObject.SetActive(false);
                rootSlotView = gridContainerView.GetSlotViewByIndex(itemSlot.rootIndex);
            }
            if (itemSlot.ItemData.IsStackable)
            {
                rootSlotView.stackAmountText.text = itemSlot.GetItemStack().Amount.ToString();
                RotateImage(imageRectTransform, itemStack.IsRotated);
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

        private void UpdatePreviewHighlight(Vector2Int gridPos, Vector2Int itemSize, Container targetContainer,
            GridContainerView targetContainerView)
        {
            targetContainerView.ClearHighlight();

            bool canPlace = targetContainer.CanPlaceItemAt(gridPos.x, gridPos.y, itemSize, itemSlot, itemSlot);

            targetContainerView.HighlightSlots(gridPos, itemSize, canPlace);
        }
        private void UpdateCurrentPreviewHighlight(SlotView targetSlot)
        {

            Vector2Int itemSize = itemSlot.GetItemStack().IsRotated ? itemSlot.ItemData.Size : InvertedSize(itemSlot.ItemData.Size);
            GenerateHighlight(targetSlot, itemSize);
        }

        private void GenerateHighlight(SlotView targetSlot, Vector2Int itemSize)
        {
            if (wantsRotate)
            {
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
                //Stack amount text is children of image and that one gets rotated so we return it to 0 so that it's orientation ignores the rotation
                stackAmountText.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                imageRectTransform.localEulerAngles = Vector3.zero;
                stackAmountText.transform.eulerAngles = Vector3.zero;

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
            rootSlotView.DestroyItemHighlight();
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
            rootSlotView.isDragging = true;
            InventorySystem.IsDragging = true;

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
                if (dropTargetSlot.itemSlot.Index == itemSlot.rootIndex && !wantsRotate && !itemSlot.ItemData.IsStackable)
                {
                    Debug.Log("Dropped on the same slot");
                    rootSlotView.CreateItemHighlight();
                    dropTargetSlot.ClearHighlight();
                    ReturnSlotState();
                    return;
                }
            }

            if (!dropTargetSlot.isEmpty)
            {
                if (dropTargetSlot.rootSlotView != rootSlotView)
                {
                    if (dropTargetSlot.itemSlot.ItemData == itemSlot.ItemData && itemSlot.ItemData.IsStackable && !dropTargetSlot.itemSlot.IsFull && !itemSlot.IsFull)
                    {
                        bool canPlace = dropTargetSlot.container.AddItemToSlot(itemSlot, dropTargetSlot.itemSlot.ContainerPosition.x, dropTargetSlot.itemSlot.ContainerPosition.y, out int remainingAmount);
                        if (canPlace)
                        {
                            if (remainingAmount > 0)
                            {
                                dropTargetSlot.container.AddItem(new ItemStack(itemSlot.ItemData,false,remainingAmount),out int currentRemainingAmount);
                                Debug.Log($"Remaining Amount {remainingAmount}, currentRemainingAmount {currentRemainingAmount}");
                            }
                            Debug.Log("Item sucessfully added");
                            container.RemoveItem(itemSlot.rootIndex);
                        }
                        else
                        {
                            ReturnSlotState();
                        }
                        ClearSlotHighlight();
                        Debug.Log("Not empty and same item and is stackable");
                        return;
                    }
                    else
                    {
                        Debug.Log("Not empty and not the same item and not stackable");
                        dropTargetSlot.rootSlotView.CreateItemHighlight();
                        ReturnSlotState();
                        return;
                    }
                }
            }
            ItemStack itemStack = itemSlot.GetItemStack();
            container.RemoveItem(itemSlot.rootIndex);
            if (dropTargetSlot.itemSlot.HasItemStack)
            {
                bool success;
                //for now return later make a way to swap if there is 1 item in the required slots
                if (dropTargetSlot.container != container)
                {
                    success = dropTargetSlot.container.AddItemToSlot(itemSlot, dropTargetSlot.itemSlot.ContainerPosition.x, dropTargetSlot.itemSlot.ContainerPosition.y, out int _);
                }
                else
                {
                    success = container.AddItemToSlot(itemSlot, dropTargetSlot.itemSlot.ContainerPosition.x, dropTargetSlot.itemSlot.ContainerPosition.y, out int _);
                }
                if (success)
                {
                    Debug.Log("Item sucessfully added");
                }
                ReturnSlotState();
                //Todo Add a way to add to stacks
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
                    dropTargetSlot.CreateItemHighlight();
                    Debug.Log("Item sucessfully added");
                    //Debug.Log(itemStack.IsRotated);
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
            rootSlotView.isDragging = false;
            InventorySystem.IsDragging = false;
        }

        private void ReturnSlotState()
        {
            wantsRotate = false;
            ClearSlotHighlight();
            rootSlotView.imageRectTransform.gameObject.SetActive(true);
            rootSlotView.imageRectTransform.localPosition = originalImagePosition;
            rootSlotView.isDragging = false;
            InventorySystem.IsDragging = false;
        }

        private void ClearSlotHighlight()
        {
            gridContainerView.ClearHighlight();
            if (currentTargetSlot != null)
            {
                currentTargetSlot.gridContainerView.ClearHighlight();
            }
        }

        private Vector2Int InvertedSize(Vector2Int size)
        {
            return new Vector2Int(size.y, size.x);
        }

        [SerializeField] private Image highlightImagePrefab;
        private Image itemHighlightImageInstance;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlightImagePrefab == null)
            {
                Debug.LogWarning("Item Highlight not set");
                return;
            }
            if (isEmpty || InventorySystem.IsDragging || rootSlotView == null)
                return;
            rootSlotView.CreateItemHighlight();

        }

        public void CreateItemHighlight()
        {
            DestroyItemHighlight();
            itemHighlightImageInstance = Instantiate(highlightImagePrefab, imageHolderTransform);
            if (itemSlot.GetItemStack().IsRotated)
                itemHighlightImageInstance.rectTransform.sizeDelta = new Vector2(rootSlotView.imageRectTransform.sizeDelta.y, rootSlotView.imageRectTransform.sizeDelta.x);
            else
                itemHighlightImageInstance.rectTransform.sizeDelta = rootSlotView.imageRectTransform.sizeDelta;
            itemHighlightImageInstance.rectTransform.SetParent(imageHolderTransform);
            itemHighlightImageInstance.rectTransform.SetAsFirstSibling();
            itemHighlightImageInstance.rectTransform.localScale = Vector2.one;
            itemHighlightImageInstance.transform.position = rootSlotView.transform.position;
        }

        public void DestroyItemHighlight()
        {
            if (itemHighlightImageInstance != null)
            {
                Destroy(itemHighlightImageInstance);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!rootSlotView)
                return;
            if (isEmpty || InventorySystem.IsDragging)
                return;
            rootSlotView.DestroyItemHighlight();
        }
    }
}
