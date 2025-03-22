using UnityEngine;
using UnityEngine.Tilemaps;

public enum FloorType
{
    Grass,
    Dirt,
    Carpet,
    Warehouse,

}



[CreateAssetMenu(fileName = "TileData", menuName = "Tiles/TileData")]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public AudioClip[] audioClip;
    public FloorType floorType;

}
