using System;

namespace InventorySystem
{
    public class ItemRootRemovedEventArgs : EventArgs
    {
        public ItemStack ItemStack { get; set; }
        
        public int RootIndex { get; set; }
    }
}