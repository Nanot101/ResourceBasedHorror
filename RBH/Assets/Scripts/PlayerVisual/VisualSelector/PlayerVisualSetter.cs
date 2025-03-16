// using UnityEngine;

// [RequireComponent(typeof(SpriteRenderer))]
// public class PlayerVisualSetter : MonoBehaviour
// {
//     [SerializeField]
//     private SpriteRenderer spriteRenderer;

//     private void Start()
//     {
//         Debug.Assert(spriteRenderer != null, $"Sprite Renderer is required for {nameof(PlayerVisualSetter)}");
//     }

//     public void SetVisual(PlayerVisual visual)
//     {
//         Debug.Assert(visual != null);

//         spriteRenderer.sprite = visual.Sprite;
//         spriteRenderer.color = visual.Color;
//     }
// }



using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisualSetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer leftShoeRenderer;
    [SerializeField] private SpriteRenderer rightShoeRenderer;

    private void Start()
    {
        Debug.Assert(bodyRenderer != null, $"Body SpriteRenderer is required for {nameof(PlayerVisualSetter)}");
        Debug.Assert(leftShoeRenderer != null, $"Left Shoe SpriteRenderer is required for {nameof(PlayerVisualSetter)}");
        Debug.Assert(rightShoeRenderer != null, $"Right Shoe SpriteRenderer is required for {nameof(PlayerVisualSetter)}");
    }

    public void SetVisual(PlayerVisual visual)
    {
        Debug.Assert(visual != null);

        bodyRenderer.sprite = visual.Sprite;
        bodyRenderer.color = visual.Color;

        leftShoeRenderer.sprite = visual.LeftShoe;
        rightShoeRenderer.sprite = visual.RightShoe;
    }
}
