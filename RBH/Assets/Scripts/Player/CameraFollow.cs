using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Tooltip("Approximately the time it will take to reach the player. A smaller value will reach the player faster.")]
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        var playerPos = player.position;

        var targetCameraPos = new Vector3(playerPos.x, playerPos.y, transform.position.z);

        transform.position =
            Vector3.SmoothDamp(transform.position, targetCameraPos, ref velocity, smoothTime, Mathf.Infinity,
                Time.unscaledDeltaTime);
    }
}