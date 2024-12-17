using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class Chest : MonoBehaviour
    {
        public ContainerHandler containerHandler;
        public GridContainerView gridContainerView;

        private void Awake()
        {
            containerHandler = GetComponent<ContainerHandler>();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (gridContainerView.IsVisible)
                    gridContainerView.HideContainer();
                else
                    gridContainerView.ShowContainer(containerHandler.Container);
            }
        }

    }
}
