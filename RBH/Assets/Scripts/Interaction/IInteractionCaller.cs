using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Marker interface for all interaction callers
/// </summary>
public interface IInteractionCaller
{
    public GameObject GameObject { get; }
}
