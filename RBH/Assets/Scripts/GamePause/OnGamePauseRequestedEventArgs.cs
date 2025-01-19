using System;

public class OnGamePauseRequestedEventArgs : EventArgs
{
    public object PauseRequestKey { get; set; }
}