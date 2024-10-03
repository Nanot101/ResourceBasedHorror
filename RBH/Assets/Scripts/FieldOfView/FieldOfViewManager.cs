using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FieldOfViewManager : Singleton<FieldOfViewManager>
{
    [SerializeField]
    [Tooltip("Render feature that renders objects invisible at night")]
    private RenderObjects invisibleAtNightRender;

    [SerializeField]
    [Tooltip("All phases on which the field of view should be active")]
    private List<DayNightPhase> phasesWithFOV = new();

    [SerializeField]
    private GameObject fieldOfView;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(invisibleAtNightRender != null, $"{nameof(invisibleAtNightRender)} is required for {nameof(FieldOfViewManager)}");

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy() => DayNightSystem.Instance.OnPhaseChanged -= OnDayNightPhaseChanged;

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (!phasesWithFOV.Contains(currentPhase))
        {
            DisableFOVSystem();
            return;
        }

        if (!enabled)
        {
            return;
        }

        EnabeFOVSystem();
    }

    private void EnabeFOVSystem()
    {
        invisibleAtNightRender.settings.stencilSettings.stencilCompareFunction = UnityEngine.Rendering.CompareFunction.Equal;
        invisibleAtNightRender.Create();

        fieldOfView.SetActive(true);
    }

    private void DisableFOVSystem()
    {
        invisibleAtNightRender.settings.stencilSettings.stencilCompareFunction = UnityEngine.Rendering.CompareFunction.Always;
        invisibleAtNightRender.Create();

        fieldOfView.SetActive(false);
    }
}
