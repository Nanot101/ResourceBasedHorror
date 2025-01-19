using System;
using System.Collections.Generic;

public static class GamePause
{
    //Use this to check if game is paused or not
    public static bool IsPaused { get; private set; }

    public static event EventHandler OnGamePaused;
    public static event EventHandler OnGameResumed;
    public static event EventHandler<OnGamePauseRequestedEventArgs> OnGamePauseRequested;
    public static event EventHandler<OnGameResumeRequestedEventArgs> OnGameResumeRequested;

    private static readonly HashSet<object> PauseRequestsKeys = new();

    public static bool RequestPause<TKey>() => RequestPause(typeof(TKey));

    public static bool RequestPause(object key)
    {
        if (!PauseRequestsKeys.Add(key))
        {
            return false;
        }

        OnGamePauseRequested?.Invoke(null, new OnGamePauseRequestedEventArgs { PauseRequestKey = key });

        TryPause();

        return true;
    }

    public static bool RequestResume<TKey>() => RequestResume(typeof(TKey));

    public static bool RequestResume(object key)
    {
        if (!PauseRequestsKeys.Remove(key))
        {
            return false;
        }

        OnGameResumeRequested?.Invoke(null, new OnGameResumeRequestedEventArgs { ResumeRequestKey = key });

        TryResume();

        return true;
    }

    public static bool IsPauseRequested<TKey>() => IsPauseRequested(typeof(TKey));

    public static bool IsPauseRequested(object key) => PauseRequestsKeys.Contains(key);

    private static void TryPause()
    {
        if (IsPaused)
        {
            return;
        }

        if (PauseRequestsKeys.Count == 0)
        {
            return;
        }

        IsPaused = true;

        OnGamePaused?.Invoke(null, EventArgs.Empty);
    }

    private static void TryResume()
    {
        if (!IsPaused)
        {
            return;
        }

        if (PauseRequestsKeys.Count > 0)
        {
            return;
        }

        IsPaused = false;

        OnGameResumed?.Invoke(null, EventArgs.Empty);
    }
}