using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public class DayNightClock : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clockText;

    [SerializeField]
    private string dayStartText = "7AM";

    [SerializeField]
    private string dayEndText = "7PM";

    [SerializeField]
    private string nightStartText = "7PM";

    [SerializeField]
    private string nightEndText = "7AM";

    [SerializeField]
    private List<DayNightPhase> dayPhases = new();

    [SerializeField]
    private List<DayNightPhase> nightPhases = new();

    private float currentPhaseMinuteDuration = 0.0f;
    private DateTime currentTime;
    private bool IsDayPhase;

    private void Awake()
    {
        AsserDesignerFields();

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void Update()
    {
        var minuteChangeSpeed = currentPhaseMinuteDuration * Time.deltaTime;

        currentTime = currentTime.AddMinutes(minuteChangeSpeed);

        UpdateUI();
    }

    private void OnDestroy()
    {
        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (dayPhases.Contains(currentPhase))
        {
            if (IsDayPhase)
            {
                return;
            }

            var dayStart = TryParseTimeText(dayStartText);
            var dayEnd = TryParseTimeText(dayEndText);

            var dayMinutes = (float)(dayEnd - dayStart).TotalMinutes;

            var dayPhasesDuration = dayPhases.Aggregate(0.0f, (acu, x) => acu += x.Duration);

            currentPhaseMinuteDuration = 1.0f / (dayPhasesDuration / dayMinutes);
            currentTime = dayStart;

            IsDayPhase = true;

            return;
        }

        if (nightPhases.Contains(currentPhase))
        {
            if (!IsDayPhase)
            {
                return;
            }

            var nightStart = TryParseTimeText(nightStartText);
            var nightEnd = TryParseTimeText(nightEndText);

            var nightMinutes = (float)(nightEnd.AddDays(1) - nightStart).TotalMinutes;

            var nightPhasesDuration = nightPhases.Aggregate(0.0f, (acu, x) => acu += x.Duration);

            currentPhaseMinuteDuration = 1.0f / (nightPhasesDuration / nightMinutes);
            currentTime = nightStart;

            IsDayPhase = false;

            return;
        }

        currentPhaseMinuteDuration = 0.0f;
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

    private void AsserDesignerFields()
    {
        if (clockText == null)
        {
            Debug.LogError("Clock text is required for day night clock");
            Destroy(this);
        }

        if (dayPhases.Count == 0)
        {
            Debug.LogError($"{name} has no {nameof(dayPhases)}. Please add at least one phase.");
            Destroy(this);
        }

        if (nightPhases.Count == 0)
        {
            Debug.LogError($"{name} has no {nameof(nightPhases)}. Please add at least one phase.");
            Destroy(this);
        }
    }
}
