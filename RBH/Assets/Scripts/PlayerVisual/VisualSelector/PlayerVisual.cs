using UnityEngine;

[CreateAssetMenu(fileName = "MyPlayerVisual", menuName = "Player Visual")]
public class PlayerVisual : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("Sprite used by SpriteRenderer in Player Visual object")]
    public Sprite Sprite {  get; set; }

    [field: SerializeField]
    [field: Tooltip("Color used by SpriteRenderer in Player Visual object")]
    public Color Color { get; set; }
}
