using UnityEngine;

public class DayNightCameraBackground : MonoBehaviour
{
    [Tooltip("Camera to change background color from, if not assigned will try to use main camera")]
    [SerializeField]
    private Camera target;

    private float currentPhaseTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        CameraSetup();

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy()
    {
        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DayNightSystem.Instance.CurrentPhase is not DayNightPhaseWithCameraBackgroundBlend currentPhase)
        {
            return;
        }

        var currentPhaseDuration = currentPhase.Duration;

        if (currentPhaseTime >= currentPhaseDuration)
        {
            return;
        }

        currentPhaseTime += Time.deltaTime;

        var blendValue = currentPhaseTime / currentPhaseDuration;

        target.backgroundColor = currentPhase.GetBlendedColor(blendValue);
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase is DayNightPhaseWithCameraBackgroundBlend phaseWithCameraBackgroundBlend)
        {
            currentPhaseTime = 0.0f;
            target.backgroundColor = phaseWithCameraBackgroundBlend.BackgroundColor;
        }
        else if (currentPhase is DayNightPhaseWithCameraBackground phaseWithCameraBackground)
        {
            target.backgroundColor = phaseWithCameraBackground.BackgroundColor;
        }
    }

    private void CameraSetup()
    {
        if (target == null)
        {
            Debug.LogWarning($"No target camera assigned to the {name}");

            target = Camera.main;

            if (!target)
            {
                Debug.LogError($"No main camera found in the scene, please assign a target camera to the {name}");
                return;
            }
        }

        target.clearFlags = CameraClearFlags.SolidColor;
    }
}
