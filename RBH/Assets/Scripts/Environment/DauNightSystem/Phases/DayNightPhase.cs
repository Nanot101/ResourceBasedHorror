using UnityEngine;

/// <summary>
/// Day night system phase
/// </summary>
public class DayNightPhase : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("Duration of this day night phase in seconds. Must be greater than zero.")]
    public float Duration { get; private set; } = 10.0f;
}
