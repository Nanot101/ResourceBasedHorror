using System;
using UnityEngine;

public class DayNightCounter : Singleton<DayNightCounter>
{
    [Tooltip("Maximum number of cycles after which game ends. It must be greater than zero")]
    [SerializeField]
    private int maxCycles = 5;

    [SerializeField]
    private DayNightPhase dayStartPhase;

    [SerializeField]
    private DayNightPhase nightStartPhase;

    public int CurrentDay { get; private set; }

    public int CurrentNight { get; private set; }

    public event EventHandler<OnNewDayArgs> OnNewDay;

    public event EventHandler<OnNewNightArgs> OnNewNight;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(maxCycles > 0, "Max days must be greater than zero");

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy() => DayNightSystem.Instance.OnPhaseChanged -= OnDayNightPhaseChanged;

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        if (CurrentNight >= maxCycles)
        {
            ResetCounters();
        }

        var currentPhase = args.CurrentPhase;

        if (currentPhase == dayStartPhase)
        {
            CurrentDay++;

            OnNewDay?.Invoke(this, new OnNewDayArgs { NewDayNumber = CurrentDay });
        }
        else if (currentPhase == nightStartPhase)
        {
            CurrentNight++;

            OnNewNight?.Invoke(this, new OnNewNightArgs { NewNightNumber = CurrentNight });
        }

        //Debug.Log($"Current day: {CurrentDay}, current night: {CurrentNight}");
    }

    private void ResetCounters()
    {
        CurrentDay = 0;
        CurrentNight = 0;
    }
}
