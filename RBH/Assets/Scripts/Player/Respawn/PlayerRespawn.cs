using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField]
    private Transform respawnPoint;

    [SerializeField]
    private PlayerMovement movement;

    [SerializeField]
    private PlayerHealthSystem healthSystem;

    [SerializeField]
    private PlayerDamageReceiver damageReceiver;

    [SerializeField]
    private PlayerStamina stamina;

    [SerializeField]
    private PlayerStaminaBar staminaBar;

    private void Start()
    {
        Debug.Assert(healthSystem != null, "Player health system is required for player respawn");
        Debug.Assert(movement != null, "Player movement is required for player respawn");
        Debug.Assert(damageReceiver != null, "Player damage reciver is required for player respawn");
        Debug.Assert(stamina != null, "Player stamina is required for player respawn");
        Debug.Assert(staminaBar != null, "Stamina bar is required for player respawn");
    }

    public void RespawnPlayer()
    {
        Debug.Assert(respawnPoint != null, "Respawn point is invalid");

        gameObject.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        healthSystem.ChangeHealth(100);

        movement.enabled = true;

        damageReceiver.OnPlayerRespawned();
        stamina.OnPlayerRespawned();
        staminaBar.OnPlayerRespawned();
    }
}
