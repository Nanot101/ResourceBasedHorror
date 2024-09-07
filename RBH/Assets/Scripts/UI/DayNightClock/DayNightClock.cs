using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayNighClock : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clockText;

    [SerializeField]
    private string dayStartText = "7AM";

    [SerializeField]
    private string nightStartText = "7PM";

    private float dayHourDuration;
    private float nightHourDuration;

    private DateTime dayStart;
    private DateTime nightStart;

    private DateTime currentTime;
    //private DayNightSystem.Cycle currentCycle;

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

    private void OnDestroy()
    {
        DayNightSystem.onDayNightSystemStarted -= OnDayNightSystemStarted;
        DayNightSystem.onCycleChange -= OnCycleChange;
    }

    private void OnDayNightSystemStarted(float dayDuration, float nightDuration, float transitionDuration)
    {
        var dayHours = (float)(nightStart - dayStart).TotalHours;
        var nightHours = (float)(dayStart.AddDays(1) - nightStart).TotalHours;

        dayHourDuration = dayDuration / dayHours;
        nightHourDuration = nightDuration / nightHours;
    }

    private void OnCycleChange(DayNightSystem.Cycle cycle)
    {
        StopAllCoroutines();

        //currentCycle = cycle;

        switch (cycle)
        {
            case DayNightSystem.Cycle.Day:

                currentTime = dayStart;

                StartCoroutine(ClockCorutine(dayHourDuration));

                break;

            case DayNightSystem.Cycle.Night:

                currentTime = nightStart;

                StartCoroutine(ClockCorutine(nightHourDuration));

                break;
        }

        UpdateUI();
    }

    private IEnumerator ClockCorutine(float hourDuration)
    {
        while (true)
        {
            yield return new WaitForSeconds(hourDuration);

            currentTime = currentTime.AddHours(1);

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        //Debug.Log($"day night clock: {currentTime:hh tt}");

        clockText.text = currentTime.ToString("hh tt");
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
