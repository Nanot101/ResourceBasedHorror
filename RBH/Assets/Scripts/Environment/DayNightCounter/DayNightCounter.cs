using System;
using UnityEngine;

public class DayNightCounter : Singleton<DayNightCounter>
{
    [Tooltip("It must be greater than zero")]
    [SerializeField]
    private int MaxDays = 5;

    public int CurrentDay { get; private set; }

    public int CurrentNight { get; private set; }

    public bool IsNight { get; private set; }

    public event EventHandler<OnNewDayArgs> OnNewDay;

    public event EventHandler<OnNewNightArgs> OnNewNight;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(MaxDays > 0, "Max days must be greater than zero");

        DayNightSystem.onCycleChange += OnDayNightCycleChange;
    }

    private void OnDestroy() => DayNightSystem.onCycleChange -= OnDayNightCycleChange;

    private void OnDayNightCycleChange(DayNightSystem.Cycle newCycle)
    {
        if (CurrentNight >= MaxDays)
        {
            ResetCounters();
        }

        switch (newCycle)
        {
            case DayNightSystem.Cycle.Day:

                CurrentDay++;

                IsNight = false;

                OnNewDay?.Invoke(this,
                    new OnNewDayArgs
                    {
                        NewDayNumber = CurrentDay
                    });

                break;

            case DayNightSystem.Cycle.Night:

                CurrentNight++;

                IsNight = true;

                OnNewNight?.Invoke(this,
                    new OnNewNightArgs
                    {
                        NewNightNumber = CurrentNight
                    });

                break;
        }

        Debug.Log($"Current day: {CurrentDay}, current night: {CurrentNight}, is night: {IsNight}");
    }

    private void ResetCounters()
    {
        CurrentDay = 0;
        CurrentNight = 0;
        IsNight = false;
    }
}
