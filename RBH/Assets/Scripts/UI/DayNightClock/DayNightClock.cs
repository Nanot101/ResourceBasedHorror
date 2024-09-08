using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DayNightClock : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clockText;

    [SerializeField]
    private string dayStartText = "7AM";

    [SerializeField]
    private string nightStartText = "7PM";

    private float dayMinuteDuration;
    private float nightMinuteDuration;

    private DateTime dayStart;
    private DateTime nightStart;

    private DateTime currentTime;
    private DayNightSystem.Cycle currentCycle;

    private void Awake()
    {
        if (clockText == null)
        {
            Debug.LogError("Clock text is required for day night clock");
            Destroy(this);
        }

        dayStart = TryParseTimeText(dayStartText);
        nightStart = TryParseTimeText(nightStartText);

        DayNightSystem.onDayNightSystemStarted += OnDayNightSystemStarted;
        DayNightSystem.onCycleChange += OnCycleChange;
    }

    private void Update()
    {
        var minuteChangeSpeed = Time.deltaTime;

        minuteChangeSpeed *= currentCycle switch
        {
            DayNightSystem.Cycle.Day => dayMinuteDuration,
            DayNightSystem.Cycle.Night => nightMinuteDuration,
            _ => 0.0f,
        };

        currentTime = currentTime.AddMinutes(minuteChangeSpeed);

        UpdateUI();
    }

    private void OnDestroy()
    {
        DayNightSystem.onDayNightSystemStarted -= OnDayNightSystemStarted;
        DayNightSystem.onCycleChange -= OnCycleChange;
    }

    private void OnDayNightSystemStarted(float dayDuration, float nightDuration, float transitionDuration)
    {
        var dayMinutes = (float)(nightStart - dayStart).TotalMinutes;
        var nightMinutes = (float)(dayStart.AddDays(1) - nightStart).TotalMinutes;

        dayMinuteDuration = 1.0f / (dayDuration / dayMinutes);
        nightMinuteDuration = 1.0f / (nightDuration / nightMinutes);
    }

    private void OnCycleChange(DayNightSystem.Cycle cycle)
    {
        currentCycle = cycle;

        switch (cycle)
        {
            case DayNightSystem.Cycle.Day:

                currentTime = dayStart;

                break;

            case DayNightSystem.Cycle.Night:

                currentTime = nightStart;

                break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        const string timeFormat = "hh:mm tt";

        //Debug.Log($"day night clock: {currentTime.ToString(timeFormat)}");

        clockText.text = currentTime.ToString(timeFormat);
    }

    private DateTime TryParseTimeText(string timeText)
    {
        if (!DateTime.TryParse(timeText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            Debug.LogError("Incorrect time format.");
            Destroy(this);
        }

        return result;
    }
}
