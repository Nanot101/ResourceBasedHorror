using UnityEngine;

public class ItemDatabaseRetriever : MonoBehaviour
{
    public ItemDatabase itemDatabase;

    /// <summary>
    /// Get item by index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ItemData GetItem(int index)
    {
        return itemDatabase.items[index];
    }

    /// <summary>
    /// Get Item by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemData GetItem(string id)
    {
        for (int i = 0; i < itemDatabase.items.Length; i++)
        {
            if (itemDatabase.items[i].id == id)
            {
                return itemDatabase.items[i];
            }
        }
        return null;
    }
}
