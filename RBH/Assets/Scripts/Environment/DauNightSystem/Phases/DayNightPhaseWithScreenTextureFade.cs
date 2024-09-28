using UnityEngine;

/// <summary>
/// Day night phase with screen Texture
/// </summary>
[CreateAssetMenu(fileName = "MyPhaseWithScreenTextureFade", menuName = "Day Night System/Phase With Screen Texture Fade")]
public class DayNightPhaseWithScreenTextureFade : DayNightPhaseWithScreenTexture
{
    [field: SerializeField]
    [field: Tooltip("Fade direction.")]
    public FadeDirection Direction { get; private set; }

    public enum FadeDirection
    {
        FadeIn,
        FadeOut
    }
}
