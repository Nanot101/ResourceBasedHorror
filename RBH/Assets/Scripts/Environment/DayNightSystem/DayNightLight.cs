using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightLight : MonoBehaviour
{
    [SerializeField] private VolumeProfile sceneGlobalProfile;

    [SerializeField] private DayNightPhase gradientZeroPercentPhase;
    [SerializeField] private DayNightPhase gradientMoveFormZeroToHundredPhase;
    [SerializeField] private DayNightPhase gradientHundredPercentPhase;
    [SerializeField] private Gradient dayNightGradient;

    private float currentPhaseTime = 0.0f;
    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        AssertDesignerFields();

        if (!sceneGlobalProfile.TryGet(out colorAdjustments))
        {
            Debug.LogError($"Did not find {nameof(ColorAdjustments)} in {nameof(sceneGlobalProfile)}");
        }

        SetColorFromGradientByTime(0.0f);

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void Update()
    {
        var currentPhase = DayNightSystem.Instance.CurrentPhase;

        if (currentPhase != gradientMoveFormZeroToHundredPhase)
        {
            return;
        }

        var currentPhaseDuration = currentPhase.Duration;

        if (currentPhaseTime >= currentPhaseDuration)
        {
            return;
        }

        currentPhaseTime += Time.deltaTime;

        var time = currentPhaseTime / currentPhaseDuration;
        SetColorFromGradientByTime(time);
    }

    private void OnDestroy()
    {
        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }

        SetColorFromGradientByTime(0.0f);
    }

    private void SetColorFromGradientByTime(float time)
    {
        colorAdjustments.colorFilter.Override(dayNightGradient.Evaluate(time));
        sceneGlobalProfile.isDirty = true;
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase == gradientZeroPercentPhase)
        {
            SetColorFromGradientByTime(0.0f);
        }
        else if (currentPhase == gradientHundredPercentPhase)
        {
            SetColorFromGradientByTime(1.0f);
        }
        else if (currentPhase == gradientMoveFormZeroToHundredPhase)
        {
            SetColorFromGradientByTime(0.0f);
            currentPhaseTime = 0.0f;
        }
    }

    private void AssertDesignerFields()
    {
        Debug.Assert(sceneGlobalProfile, $"{nameof(sceneGlobalProfile)} is required for {nameof(DayNightLight)}",
            this);
        Debug.Assert(gradientZeroPercentPhase,
            $"{nameof(gradientZeroPercentPhase)} is required for {nameof(DayNightLight)}", this);
        Debug.Assert(gradientMoveFormZeroToHundredPhase,
            $"{nameof(gradientMoveFormZeroToHundredPhase)} is required for {nameof(DayNightLight)}", this);
        Debug.Assert(gradientHundredPercentPhase,
            $"{nameof(gradientHundredPercentPhase)} is required for {nameof(DayNightLight)}",
            this);
    }
}