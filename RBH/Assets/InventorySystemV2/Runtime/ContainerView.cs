using System;
using System.Collections.Generic;
using UnityEngine;
namespace InventorySystem
{
    public class ContainerView : MonoBehaviour
    {
        [SerializeField] private SlotView slotViewPrefab;
        [SerializeField] private Transform slotContent;

        public Transform SlotContent { get; }

        protected readonly List<SlotView> slots = new List<SlotView>();

        public bool IsVisible { get; private set; }
        public virtual void ShowContainer(Container container)
        {
            slots.DestroyAllObjectsInList();
            slots.Clear();
            for (int i = 0; i < container.SlotCount; i++)
            {
                SlotView slotView = Instantiate(slotViewPrefab, slotContent);
                slots.Add(slotView);
            }
            gameObject.SetActive(true);
            IsVisible = true;
        }

        public void HideContainer()
        {
            IsVisible = false;
            gameObject.SetActive(false);
        }
    }
}
