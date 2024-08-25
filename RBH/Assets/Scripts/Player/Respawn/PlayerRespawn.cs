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
        if (healthSystem == null)
        {
            Debug.LogError("Player health system is required for player respawn");
            Destroy(this);
        }

        if (movement == null)
        {
            Debug.LogError("Player movement is required for player respawn");
            Destroy(this);
        }

        if (damageReceiver == null)
        {
            Debug.LogError("Player damage reciver is required for player respawn");
            Destroy(this);
        }

        if (stamina == null)
        {
            Debug.LogError("Player stamina is required for player respawn");
            Destroy(this);
        }

        if (staminaBar == null)
        {
            Debug.LogError("Stamina bar is required for player respawn");
            Destroy(this);
        }
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
