using Unity.Mathematics;
using UnityEngine;

public class FieldOfViewSettingsBlending : IFieldOfViewSettings
{
    private readonly IFieldOfViewSettings _firstSettings;
    private readonly IFieldOfViewSettings _secondSettings;

    private readonly float _phaseDuration;
    private float _elapsedTime;

    public float FrontViewAngle => math.lerp(_firstSettings.FrontViewAngle, _secondSettings.FrontViewAngle, GetBlendTime());

    public float FrontViewDistance => math.lerp(_firstSettings.FrontViewDistance, _secondSettings.FrontViewDistance, GetBlendTime());

    public int AdditionalFrontRayCount => (int)math.lerp(_firstSettings.AdditionalFrontRayCount, _secondSettings.AdditionalFrontRayCount, GetBlendTime());

    public float AroundViewDistance => math.lerp(_firstSettings.AroundViewDistance, _secondSettings.AroundViewDistance, GetBlendTime());

    public int AdditionalAroundRayCount => (int)math.lerp(_firstSettings.AdditionalAroundRayCount, _secondSettings.AdditionalAroundRayCount, GetBlendTime());

    public FieldOfViewSettingsBlending(IFieldOfViewSettings firstSettings, IFieldOfViewSettings secondSettings)
    {
        _firstSettings = firstSettings;
        _secondSettings = secondSettings;
        _elapsedTime = 0.0f;

        _phaseDuration = DayNightSystem.Instance.CurrentPhase.Duration;
    }

    public void Update()
    {
        _elapsedTime += Time.deltaTime;
    }

    private float GetBlendTime()
    {
        if (_elapsedTime == 0.0f)
        {
            return 0.0f;
        }

        return _elapsedTime / _phaseDuration;
    }
}
