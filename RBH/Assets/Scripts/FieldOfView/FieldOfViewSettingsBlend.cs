using UnityEngine;

[CreateAssetMenu(fileName = "MyFOVSettingsBlend", menuName = "Field Of View/Settings Blend")]
public class FieldOfViewSettingsBlend : FieldOfViewSettingsBase
{
    public FieldOfViewSettings FirstSettings;

    public FieldOfViewSettings SecondSettings;

    public override IFieldOfViewSettings CreateSettings() => new FieldOfViewSettingsBlending(FirstSettings, SecondSettings);
}
