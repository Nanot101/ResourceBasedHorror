using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        PlayerSpawnManager.Instance.OnPlayerSpawn += OnPlayerSpawn;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    private void OnDestroy()
    {
        PlayerSpawnManager.Instance.OnPlayerSpawn -= OnPlayerSpawn;
    }

    private void OnPlayerSpawn(object sender, OnPlayerSpawnArgs args)
    {
        player = args.SpawnedPlayer.transform;
    }
}