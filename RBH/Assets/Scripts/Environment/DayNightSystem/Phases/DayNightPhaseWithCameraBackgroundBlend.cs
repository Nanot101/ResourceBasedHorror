using UnityEngine;

/// <summary>
/// Day night phase with two colors of camera background with blending.
/// </summary>
[CreateAssetMenu(fileName = "MyPhaseWithCameraBackgroundBlend", menuName = "Day Night System/Phase With Camera Background Blend")]
public class DayNightPhaseWithCameraBackgroundBlend : DayNightPhaseWithCameraBackground
{
    [field: SerializeField]
    [field: Tooltip("Second background color of the camera that will be blended with first background during this phase.")]
    public Color SecondBackgroundColor { get; private set; } = Color.blue;

    public Color GetBlendedColor(float t) => Color.Lerp(BackgroundColor, SecondBackgroundColor, t);
}
