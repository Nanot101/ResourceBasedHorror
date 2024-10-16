using System.Collections.Generic;
using UnityEngine;

public abstract class FieldOfViewSettingsBase : ScriptableObject
{
    [field: SerializeField]
    [field: Tooltip("The phases of the day and night in which these settings are active")]
    public List<DayNightPhase> Phases { get; set; } = new();

    public abstract IFieldOfViewSettings CreateSettings();
}
