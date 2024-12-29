using System;

namespace InventorySystem
{
    [Serializable]
    public abstract class ItemAction
    {
        public abstract string ActionName { get;}
        public static Action actionExecuted;
        public abstract void Execute(ItemSlot itemSlot);
    }

    public class UseAction : ItemAction
    {
        private string actionName = "Use";
        public override string ActionName => actionName;

        public override void Execute(ItemSlot itemSlot)
        {
            actionExecuted?.Invoke();
        }
    }
    public class RemoveAction : ItemAction
    {
        private string actionName = "Remove";
        public override string ActionName => actionName;

        public override void Execute(ItemSlot itemSlot)
        {
            itemSlot.Container.RemoveItem(itemSlot.rootIndex);
            actionExecuted?.Invoke();

        }
    }

    public interface IItemAction
    {
        string ActionName { get;}
        void Execute(ItemSlot itemSlot);
    }
}
