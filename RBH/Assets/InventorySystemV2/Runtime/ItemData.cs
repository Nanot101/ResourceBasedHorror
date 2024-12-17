using Sirenix.OdinInspector;
using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField,ReadOnly,HorizontalGroup("RowId")]
        private string id;
        [SerializeField,HorizontalGroup("RowName")]
        private string itemName;
        [SerializeField]
        private string description;
        [SerializeField,PreviewField(100)]
        private Sprite itemSprite;
        [SerializeField]
        private Vector2Int size;
        [SerializeField]
        private int maxStackSize;
        [SerializeField]
        private string[] tags;

        [SerializeReference]
        private ItemType[] itemType;
        [SerializeReference]
        private ItemAction[] action;

        public string Id => id;
        public string ItemName => itemName;
        public string Description => description;
        public Sprite ItemSprite => itemSprite;
        public Vector2Int Size => size;
        public int MaxStackSize => maxStackSize;
        public string[] Tags => tags;
        public bool IsStackable => maxStackSize > 1;
        public ItemType[] ItemType => itemType;

        public ItemAction[] ItemAction => action;

        [HorizontalGroup("RowId",Width = 130),Button("Generate New Id")]
        public void GenerateUniqueId()
        {
            id = System.Guid.NewGuid().ToString();
        }

#if UNITY_EDITOR
        private bool renameScheduled = false;
        [HorizontalGroup("RowName",Width = 130),Button("Change File Name")]
        private void ScheduleRename()
        {
            if (!renameScheduled)
            {
                renameScheduled = true;
                UnityEditor.EditorApplication.delayCall += RenameAssetFile;
            }
        }

        private void RenameAssetFile()
        {
            renameScheduled = false;

            string path = UnityEditor.AssetDatabase.GetAssetPath(this);
            if (!string.IsNullOrEmpty(path))
            {
                string newFileName = $"{itemName}.asset";
                string newPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), newFileName);

                if (path != newPath)
                {
                    UnityEditor.AssetDatabase.RenameAsset(path, itemName);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
        }
#endif

    }

}
