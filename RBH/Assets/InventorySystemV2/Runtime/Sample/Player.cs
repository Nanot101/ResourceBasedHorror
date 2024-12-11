using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InventorySystem
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private ContainerHandler myInventoryContainer;
        [SerializeField] private ContainerView containerView;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (containerView.IsVisible)
                    containerView.HideContainer();
                else
                    containerView.ShowContainer(myInventoryContainer.Container);
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

            }
        }

        public void AddItem(int index)
        {
            myInventoryContainer.Container.AddItem(myInventoryContainer.initialItems[index],out int _);
        }
        private void RemoveItem(ItemStack itemStack)
        {

        }
    }
}
