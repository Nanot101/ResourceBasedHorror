using System;
using System.Collections;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    [SerializeField] Camera cam;

    [Header("Day")]
    [Tooltip("Duration of the day in seconds")]
    [SerializeField] float dayDuration = 60;
    [SerializeField] Color dayColor = Color.yellow;

    [Header("Night")]
    [Tooltip("Duration of the night in seconds")]
    [SerializeField] float nightDuration = 60;
    [SerializeField] Color nightColor = Color.blue;

    [Header("Transition")]
    [Tooltip("Duration of the transition in seconds")]
    [SerializeField] float transitionDuration = 10;

    [Header("Debbuging")]
    [SerializeField] bool debugMode = false;

    private Color targetColor;
    private float currentCycleDuration;

    public enum Cycle
    {
        Day,
        Transition,
        Night
    }
    //Can be expanded to other classes to listen to the cycle change, right now it is only used as a debbuging tool
    public Cycle currentCycle;

    public static Action<Cycle> onTransitionWarning;
    public static Action<Cycle> onCycleChange;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = dayColor;
        currentCycle = Cycle.Day;
        targetColor = nightColor;
        currentCycleDuration = nightDuration;

        StartCoroutine(CycleDayNight());
    }

    private IEnumerator CycleDayNight()
    {
        yield return new WaitForSeconds(currentCycleDuration - transitionDuration);
        StartCoroutine(Transition());
    }
    private IEnumerator Transition()
    {
        onTransitionWarning?.Invoke(currentCycle);
        DebugMessage("Starting day night cycle transition");
        currentCycle = Cycle.Transition;
        float transitionStartTime = Time.time;
        float elapsedTime = 0f;
        Color initialColor = cam.backgroundColor;

        while (elapsedTime < transitionDuration) //Transitions happens here
        {
            elapsedTime = Time.time - transitionStartTime;
            cam.backgroundColor = Color.Lerp(initialColor, targetColor, elapsedTime / transitionDuration);
            yield return null;
        }
        DebugMessage("Day night cycle transition ended");
        SwitchCycle();
        onCycleChange?.Invoke(currentCycle);
        StartCoroutine(CycleDayNight());
    }
    private void SwitchCycle()
    {
        if (targetColor == dayColor)
        {
            currentCycle = Cycle.Day;
            currentCycleDuration = nightDuration;
            targetColor = nightColor;
        }
        else
        {
            currentCycle = Cycle.Night;
            currentCycleDuration = dayDuration;
            targetColor = dayColor;
        }
    }

    private void DebugMessage(string message)
    {
        if (!debugMode)
            return;
        Debug.Log(message);
    }
    [ContextMenu("Debug Day-Night Transition")]
    private void DebugTransition()
    {
        Debug.Log("Manually triggering day night transition.");
        StopAllCoroutines();
        StartCoroutine(Transition());
    }
}
