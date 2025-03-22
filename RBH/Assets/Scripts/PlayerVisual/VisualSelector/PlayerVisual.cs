// using UnityEngine;

// [CreateAssetMenu(fileName = "MyPlayerVisual", menuName = "Player Visual")]
// public class PlayerVisual : ScriptableObject
// {
//     [field: SerializeField]
//     [field: Tooltip("Sprite used by SpriteRenderer in Player Visual object")]
//     public Sprite Sprite {  get; set; }

//     [field: SerializeField]
//     [field: Tooltip("Color used by SpriteRenderer in Player Visual object")]
//     public Color Color { get; set; }
// }



using UnityEngine;

[CreateAssetMenu(fileName = "MyPlayerVisual", menuName = "Player Visual")]
public class PlayerVisual : ScriptableObject
{
    [field: SerializeField, Tooltip("Sprite used for the player's body")]
    public Sprite Sprite { get; set; }

    [field: SerializeField, Tooltip("Color for the player's body")]
    public Color Color { get; set; }

    [field: SerializeField, Tooltip("Sprite used for the player's left shoe")]
    public Sprite LeftShoe { get; set; }

    [field: SerializeField, Tooltip("Sprite used for the player's right shoe")]
    public Sprite RightShoe { get; set; }
}
