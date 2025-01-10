using System;

namespace InventorySystem
{
    public class ItemPlacedAtEventArgs : EventArgs
    {
        public ItemStack ItemStack { get; set; }

        public int RootIndex { get; set; }
    }
}