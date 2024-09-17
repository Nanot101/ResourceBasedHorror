using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [ScriptableObjectId]
    public string id;

    public int width = 1;
    public int height = 1;

    public Sprite itemIcon;

    public string itemName;
}
