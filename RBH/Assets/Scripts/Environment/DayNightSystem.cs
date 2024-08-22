using System;
using System.Collections;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    #region Fields
    [Tooltip("Camera to change background color from, if not assigned will try to use main camera")]
    [SerializeField] Camera target;

    [Header("Day")]
    [Tooltip("Duration of the day in seconds")]
    [SerializeField] float dayDuration = 60;
    [SerializeField] Color dayColor = Color.yellow;

    [Header("Night")]
    [Tooltip("Duration of the night in seconds")]
    [SerializeField] float nightDuration = 60;
    [SerializeField] Color nightColor = Color.blue;

    [Header("Transition")]
    [Tooltip("Duration of the transition in seconds (part of the total duration of the cycle)")]
    [SerializeField] float transitionDuration = 10;
    private Color targetTransitionColor;
    private float currentCycleDuration;

    [Header("Cycle")]
    [Tooltip("Cycle the system is currently in")]
    public Cycle currentCycle = Cycle.Day;

    [Header("Debbuging")]
    [SerializeField] bool debugMode = false;
    #endregion
    #region Enums
    public enum Cycle
    {
        Day,
        Night
    }
    #endregion
    #region Events
    public static Action<Cycle> onTransitionWarning;
    public static Action<Cycle> onCycleChange;
    #endregion
    private void Start()
    {
        if (target == null)
        {
            DebugMessage(DebugType.Warning, "No target camera assigned to the DayNightSystem");
            if (!Camera.main)
            {
                DebugMessage(DebugType.Error, "No main camera found in the scene, please assign a target camera to the DayNightSystem");
                return;
            }
            target = Camera.main;
        }
        SetCycle(currentCycle);

        StartCoroutine(CycleDayNight());
    }

    private IEnumerator CycleDayNight()
    {
        yield return new WaitForSeconds(currentCycleDuration - transitionDuration);
        StartCoroutine(StartTransition());
    }

    private IEnumerator StartTransition()
    {
        onTransitionWarning?.Invoke(currentCycle);

        DebugMessage(DebugType.Message,"Starting day night-cycle transition");
        yield return StartCoroutine(Transition());
        DebugMessage(DebugType.Message, "Day-night cycle transition ended");
    }

    private IEnumerator Transition()
    {
        float transitionStartTime = Time.time;
        float elapsedTime = 0f;
        Color initialColor = target.backgroundColor;

        while (elapsedTime < transitionDuration) //Transitions happens here
        {
            elapsedTime = Time.time - transitionStartTime;
            target.backgroundColor = Color.Lerp(initialColor, targetTransitionColor, elapsedTime / transitionDuration);
            yield return null;
        }

        SwitchCycle();
        onCycleChange?.Invoke(currentCycle);
        StartCoroutine(CycleDayNight());
    }
    private void SwitchCycle()
    {
        if (currentCycle != Cycle.Day)
        {
            currentCycle = Cycle.Day;
            currentCycleDuration = dayDuration;
            targetTransitionColor = nightColor;
        }
        else
        {
            currentCycle = Cycle.Night;
            currentCycleDuration = nightDuration;
            targetTransitionColor = dayColor;
        }
    }

    private void SetCycle(Cycle cycle)
    {
        target.clearFlags = CameraClearFlags.SolidColor;
        switch (cycle)
        {
            case Cycle.Day:
                target.backgroundColor = dayColor;
                currentCycle = Cycle.Day;
                currentCycleDuration = dayDuration;
                targetTransitionColor = nightColor;
                break;
            case Cycle.Night:
                target.backgroundColor = nightColor;
                currentCycle = Cycle.Night;
                currentCycleDuration = nightDuration;
                targetTransitionColor = dayColor;
                break;
        }
        
    }
    #region Debugging
    enum DebugType
    {
        Message,
        Warning,
        Error
    }
    private void DebugMessage(DebugType debugType, string message)
    {
        if (!debugMode) return;

        switch (debugType)
        {
            case DebugType.Message:
                Debug.Log(message);
                break;
            case DebugType.Warning:
                Debug.LogWarning(message);
                break;
            case DebugType.Error:
                Debug.LogError(message);
                break;
        }
    }

    [ContextMenu("Debug Day-Night Transition")]
    private void DebugTransition()
    {
        if (target == null)
        {
            DebugMessage(DebugType.Error, "Target Camera was not assigned, assign it or setup a main camera before debugging");
        }
        Debug.Log("Manually triggering day-night transition.");
        StopAllCoroutines();
        StartCoroutine(Transition());
    }
    #endregion
}
