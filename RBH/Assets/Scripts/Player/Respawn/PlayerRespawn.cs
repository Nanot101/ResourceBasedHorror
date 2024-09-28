using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement movement;

    [SerializeField]
    private PlayerHealthSystem healthSystem;

    [SerializeField]
    private PlayerDamageReceiver damageReceiver;

    [SerializeField]
    private PlayerStamina stamina;

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private DayNightPhase moveToRespawnPhase;

    [SerializeField]
    private DayNightPhase respawnPhase;

    private void Start()
    {
        AssertComponents();

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy() => DayNightSystem.Instance.OnPhaseChanged -= OnDayNightPhaseChanged;

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase == moveToRespawnPhase)
        {
            TryMovePlayerToRespawnPoint();
        }
        else if (currentPhase == respawnPhase)
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        StopAllCoroutines();

        TryMovePlayerToRespawnPoint();

        healthSystem.ChangeHealth(100);

        movement.enabled = true;

        damageReceiver.OnPlayerRespawned();
        stamina.OnPlayerRespawned();
        weapon.canShoot = true;
    }

    private void TryMovePlayerToRespawnPoint()
    {
        if (PlayerRespawnPoint.Instance == null)
        {
            Debug.LogError("Instance of PlayerRespawnPoint not found. Please place PlayerRespawnPoint on the scene.");

            return;
        }

        PlayerRespawnPoint.Instance.MovePlayerObjectToRespawnPoint(gameObject);
    }

    private void AssertComponents()
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

        if (stamina == null)
        {
            Debug.LogError("Player weapon is required for player respawn");
            Destroy(this);
        }
    }
}
