using System;
using UnityEngine;

public class PlayerSpawnManager : Singleton<PlayerSpawnManager>
{
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject playerPrefab;

    public event EventHandler<OnPlayerSpawnArgs> OnPlayerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spawnPoint != null, "Spawn must be set");
        Debug.Assert(playerPrefab != null, "Player prefab must be set");

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        var player = Instantiate(playerPrefab, transform.position, transform.rotation);

        var args = new OnPlayerSpawnArgs
        {
            SpawnedPlayer = player
        };

        OnPlayerSpawn?.Invoke(this, args);
    }
}
