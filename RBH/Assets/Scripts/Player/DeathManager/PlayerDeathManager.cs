using System;
using UnityEngine;

public class PlayerDeathManager : Singleton<PlayerDeathManager>
{
    [SerializeField]
    private GameOverUI gameOverUI;

    private GameObject currentPlayer;

    public event EventHandler OnPlayerRespawn;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(gameOverUI != null, "Game over UI must be set");

        gameOverUI.AddActionToRespawnButton(RespawnPlayer);

        PlayerHealthSystem.onPlayerDied += OnPlayerDied;
        PlayerSpawnManager.Instance.OnPlayerSpawn += OnPlayerSpawn;
    }

    private void OnDestroy()
    {
        PlayerHealthSystem.onPlayerDied -= OnPlayerDied;
        PlayerSpawnManager.Instance.OnPlayerSpawn -= OnPlayerSpawn;
    }

    private void RespawnPlayer()
    {
        Destroy(currentPlayer);

        currentPlayer = null;

        gameOverUI.Hide();

        PlayerSpawnManager.Instance.SpawnPlayer();

        OnPlayerRespawn?.Invoke(this, EventArgs.Empty);
    }

    private void OnPlayerDied()
    {
        gameOverUI.Show();
    }

    private void OnPlayerSpawn(object sender, OnPlayerSpawnArgs args)
    {
        currentPlayer = args.SpawnedPlayer;
    }
}
