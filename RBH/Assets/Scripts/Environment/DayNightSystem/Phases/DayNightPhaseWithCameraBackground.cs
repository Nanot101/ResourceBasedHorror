using UnityEngine;

/// <summary>
/// Day night phase with camera background
/// </summary>
[CreateAssetMenu(fileName = "MyPhaseWithCameraBackground", menuName = "Day Night System/Phase With Camera Background")]
public class DayNightPhaseWithCameraBackground : DayNightPhase
{
    [field: SerializeField]
    [field: Tooltip("Background color for the camera to be used during this phase.")]
    public Color BackgroundColor { get; private set; } = Color.yellow;
}
