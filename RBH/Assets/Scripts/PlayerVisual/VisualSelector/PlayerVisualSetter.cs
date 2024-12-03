using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisualSetter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        Debug.Assert(spriteRenderer != null, $"Sprite Renderer is required for {nameof(PlayerVisualSetter)}");
    }

    public void SetVisual(PlayerVisual visual)
    {
        Debug.Assert(visual != null);

        spriteRenderer.sprite = visual.Sprite;
        spriteRenderer.color = visual.Color;
    }
}
