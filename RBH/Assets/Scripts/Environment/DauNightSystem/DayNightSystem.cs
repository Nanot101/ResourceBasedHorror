using System;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSystem : Singleton<DayNightSystem>
{
    [SerializeField]
    [Tooltip("All phases of day and night. They will be switched in a loop starting from the first phase. There must be at least one phase.")]
    private List<DayNightPhase> dayNightPhases = new();

    private int currentPhaseIndex = 0;
    private float currentPhaseTime = 0.0f;

    public DayNightPhase CurrentPhase => dayNightPhases[currentPhaseIndex];

    public event EventHandler<DayNightSystemEventArgs> OnDayNightStarted;
    public event EventHandler<DayNightSystemEventArgs> OnPhaseChanged;

    private void Start()
    {
        AssertDesignerFields();

        StartPhases();
    }

    private void Update()
    {
        if (currentPhaseTime < CurrentPhase.Duration)
        {
            currentPhaseTime += Time.deltaTime;
            return;
        }

        MoveToNextPhase();
    }

    private void MoveToNextPhase()
    {
        IncrementPhaseIndex();

        currentPhaseTime = 0.0f;

        OnPhaseChanged?.Invoke(this, new DayNightSystemEventArgs { CurrentPhase = CurrentPhase });

        //Debug.Log($"Day night system moved to {CurrentPhase.name}");
    }

    private void StartPhases()
    {
        currentPhaseIndex = 0;
        currentPhaseTime = 0.0f;

        OnDayNightStarted?.Invoke(this, new DayNightSystemEventArgs { CurrentPhase = CurrentPhase });
        OnPhaseChanged?.Invoke(this, new DayNightSystemEventArgs { CurrentPhase = CurrentPhase });

        //Debug.Log($"Day night system started with {CurrentPhase.name}");
    }

    private void IncrementPhaseIndex()
    {
        currentPhaseIndex++;

        if (currentPhaseIndex >= dayNightPhases.Count)
        {
            currentPhaseIndex = 0;
        }
    }

    private void AssertDesignerFields()
    {
        if (dayNightPhases.Count == 0)
        {
            Debug.LogError($"{name} has no phases. Please add at least one phase.");
            Destroy(this);
        }
    }

    [ContextMenu("Move To Next Phase")]
    private void MoveToNextPhaseManual() => MoveToNextPhase();

    //#region Fields
    //[Tooltip("Camera to change background color from, if not assigned will try to use main camera")]
    //[SerializeField] Camera target;

    //[Header("Day")]
    //[Tooltip("Duration of the day in seconds")]
    //[SerializeField] float dayDuration = 60;
    //[SerializeField] Color dayColor = Color.yellow;

    //[Header("Night")]
    //[Tooltip("Duration of the night in seconds")]
    //[SerializeField] float nightDuration = 60;
    //[SerializeField] Color nightColor = Color.blue;

    //[Header("Transition")]
    //[Tooltip("Duration of the transition from day to night in seconds (part of the total duration of the cycle)")]
    //[SerializeField] float dayToNightTransitionDuration = 10;

    //[Tooltip("Duration of the transition from night to day in seconds (not part of the total duration of the cycle)")]
    //[SerializeField] float nightToDayTransitionDuration = 10;

    //[Header("Cycle")]
    //[Tooltip("Cycle the system is currently in")]
    //public Cycle currentCycle = Cycle.Day;

    //[Header("Debbuging")]
    //[SerializeField] bool debugMode = false;
    //#endregion
    //#region Enums
    //public enum Cycle
    //{
    //    Day,
    //    Night,
    //    NightToDay
    //}
    //#endregion
    //#region Events
    //public static Action<Cycle> onTransitionWarning;
    //public static Action<Cycle> onCycleChange;
    //public static Action<float, float, float, float> onDayNightSystemStarted;
    //#endregion
    //private void Start()
    //{
    //    if (target == null)
    //    {
    //        DebugMessage(DebugType.Warning, "No target camera assigned to the DayNightSystem");
    //        if (!Camera.main)
    //        {
    //            Debug.LogError("No main camera found in the scene, please assign a target camera to the DayNightSystem");
    //            return;
    //        }
    //        target = Camera.main;
    //    }

    //    target.clearFlags = CameraClearFlags.SolidColor;

    //    onDayNightSystemStarted?.Invoke(dayDuration, nightDuration, dayToNightTransitionDuration, nightToDayTransitionDuration);

    //    RestartSystemFromCycle(currentCycle);
    //}

    //private void RestartSystemFromCycle(Cycle initialCycle)
    //{
    //    StopAllCoroutines();

    //    switch (initialCycle)
    //    {
    //        case Cycle.Day:
    //            StartCoroutine(DayCycle());
    //            break;

    //        case Cycle.Night:
    //            StartCoroutine(NightCycle());
    //            break;

    //        case Cycle.NightToDay:
    //            StartCoroutine(NightToDayTransition());
    //            break;
    //    }
    //}

    //private IEnumerator DayCycle()
    //{
    //    DebugMessage(DebugType.Message, "Day started");

    //    target.backgroundColor = dayColor;
    //    currentCycle = Cycle.Day;
    //    onCycleChange?.Invoke(currentCycle);

    //    yield return new WaitForSeconds(dayDuration - dayToNightTransitionDuration);

    //    DebugMessage(DebugType.Message, "Day ended");

    //    yield return DayToNightTransition();
    //}

    //private IEnumerator DayToNightTransition()
    //{
    //    DebugMessage(DebugType.Message, "Day to night started");

    //    onTransitionWarning?.Invoke(currentCycle);

    //    float transitionStartTime = Time.time;
    //    float elapsedTime = 0f;

    //    while (elapsedTime < dayToNightTransitionDuration)
    //    {
    //        elapsedTime = Time.time - transitionStartTime;

    //        target.backgroundColor = Color.Lerp(dayColor, nightColor, elapsedTime / dayToNightTransitionDuration);

    //        yield return null;
    //    }

    //    DebugMessage(DebugType.Message, "Day to night ended");

    //    yield return NightCycle();
    //}

    //private IEnumerator NightCycle()
    //{
    //    DebugMessage(DebugType.Message, "Night started");

    //    target.backgroundColor = nightColor;
    //    currentCycle = Cycle.Night;
    //    onCycleChange?.Invoke(currentCycle);

    //    yield return new WaitForSeconds(nightDuration);

    //    DebugMessage(DebugType.Message, "Night ended");

    //    yield return NightToDayTransition();

    //}

    //private IEnumerator NightToDayTransition()
    //{
    //    DebugMessage(DebugType.Message, "Night to day started");

    //    onTransitionWarning?.Invoke(currentCycle);

    //    currentCycle = Cycle.NightToDay;
    //    onCycleChange?.Invoke(currentCycle);

    //    yield return new WaitForSeconds(nightToDayTransitionDuration);

    //    DebugMessage(DebugType.Message, "Night to day ended");

    //    yield return DayCycle();
    //}

    //#region Debugging
    //enum DebugType
    //{
    //    Message,
    //    Warning,
    //}
    //private void DebugMessage(DebugType debugType, string message)
    //{
    //    if (!debugMode) return;

    //    switch (debugType)
    //    {
    //        case DebugType.Message:
    //            Debug.Log(message);
    //            break;
    //        case DebugType.Warning:
    //            Debug.LogWarning(message);
    //            break;
    //    }
    //}

    //[ContextMenu("Debug Day-Night Transition")]
    //private void DebugTransition()
    //{
    //    if (target == null)
    //    {
    //        Debug.LogError("Target Camera was not assigned, assign it or setup a main camera before debugging");
    //    }
    //    Debug.Log("Manually triggering day-night transition.");

    //    RestartSystemFromCycle(GetNextCycle(currentCycle));
    //}

    //private Cycle GetNextCycle(Cycle cycle) => cycle switch
    //{
    //    Cycle.Day => Cycle.Night,
    //    Cycle.Night => Cycle.NightToDay,
    //    Cycle.NightToDay => Cycle.Day,
    //    _ => Cycle.Day,
    //};

    //#endregion
}
