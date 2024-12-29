using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName ="Inventory/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemData> items = new List<ItemData>();
        [FolderPath]
        public string itemsFolder = "Assets/Items";
    }
}
