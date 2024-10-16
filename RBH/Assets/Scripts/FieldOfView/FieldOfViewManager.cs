using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;

public class FieldOfViewManager : Singleton<FieldOfViewManager>
{
    [SerializeField]
    [Tooltip("Render feature that renders objects visible in FOV")]
    private RenderObjects visibleInFOVRender;

    [SerializeField]
    private FieldOfView fieldOfView;

    [SerializeField]
    private List<FieldOfViewSettingsBase> settings = new();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(visibleInFOVRender != null, $"{nameof(visibleInFOVRender)} is required for {nameof(FieldOfViewManager)}");
        Debug.Assert(fieldOfView != null, $"{nameof(fieldOfView)} is required for {nameof(FieldOfViewManager)}");

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;

        EnabeFOVSystem();
    }

    private void OnDestroy()
    {
        DisableFOVSystem();
        DayNightSystem.Instance.OnPhaseChanged -= OnDayNightPhaseChanged;
    }

    private void OnEnable() => EnabeFOVSystem();

    private void OnDisable() => DisableFOVSystem();

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        if (!enabled)
        {
            return;
        }

        var currentPhase = args.CurrentPhase;

        var settingsForThisPhase = settings
            .Where(x =>
                x.Phases
                .Contains(currentPhase))
            .Select(x => x.CreateSettings())
            .FirstOrDefault();

        fieldOfView.SetNextSettings(settingsForThisPhase);
    }

    private void EnabeFOVSystem()
    {
        visibleInFOVRender.settings.stencilSettings.stencilCompareFunction = CompareFunction.Equal;
        visibleInFOVRender.Create();
    }

    private void DisableFOVSystem()
    {
        visibleInFOVRender.settings.stencilSettings.stencilCompareFunction = CompareFunction.Always;
        visibleInFOVRender.Create();
    }
}
