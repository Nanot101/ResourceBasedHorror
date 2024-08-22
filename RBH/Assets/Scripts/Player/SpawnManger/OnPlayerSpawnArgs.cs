using System;
using UnityEngine;

public class OnPlayerSpawnArgs : EventArgs
{
    public GameObject SpawnedPlayer { get; set; }
}
