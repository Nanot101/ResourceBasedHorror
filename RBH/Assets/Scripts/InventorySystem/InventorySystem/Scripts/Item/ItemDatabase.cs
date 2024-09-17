using UnityEngine;
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] items;

    public ItemData FindItemInDatabase(string itemId)
    {
        foreach (ItemData item in items)
        {
            if (item.id == itemId)
            {
                return item;
            }
        }

        return null;
    }
}
