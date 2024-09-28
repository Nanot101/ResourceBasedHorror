using UnityEngine;

public class PlayerRespawnPoint : Singleton<PlayerRespawnPoint>
{
    [SerializeField]
    private Transform respawnPoint;

    private void Start()
    {
        if (respawnPoint == null)
        {
            Debug.LogError("Respawn point is required for player respawn");
            Destroy(this);
        }
    }

    public void MovePlayerObjectToRespawnPoint(GameObject playerObject)
    {
        Debug.Assert(respawnPoint != null, "Respawn point is invalid");

        playerObject.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
    }
}
