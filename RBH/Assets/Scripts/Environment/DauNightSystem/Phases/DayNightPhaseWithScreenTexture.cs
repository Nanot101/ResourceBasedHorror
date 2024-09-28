using UnityEngine;

/// <summary>
/// Day night phase with screen Texture
/// </summary>
[CreateAssetMenu(fileName = "MyPhaseWithScreenTexture", menuName = "Day Night System/Phase With Screen Texture")]
public class DayNightPhaseWithScreenTexture : DayNightPhase
{
    [field: SerializeField]
    [field: Tooltip("Texture that will cover the screen during this phase. If left empty, default black texture will be used.")]
    private Texture2D texture;

    public Texture2D Texture
    {
        get
        {
            if (texture == null)
            {
                Debug.Log($"Seting default black texture for {name}");

                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.black);
                texture.Apply();
            }

            return texture;
        }
    }
}
