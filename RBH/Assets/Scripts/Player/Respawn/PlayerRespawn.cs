using UnityEngine;
using UnityEngine.Audio;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

    [SerializeField] private PlayerHealthSystem healthSystem;

    [SerializeField] private PlayerDamageReceiver damageReceiver;

    [SerializeField] private PlayerStamina stamina;

    [SerializeField] private WeaponController weaponController;

    [SerializeField] private DayNightPhase moveToRespawnPhase;

    [SerializeField] private DayNightPhase respawnPhase;

    [SerializeField] private AudioMixer mainMixer;

    private void Start()
    {
        AssertComponents();

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;

        PlayerHealthSystem.onPlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }

        PlayerHealthSystem.onPlayerDied -= OnPlayerDied;
    }

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
        weaponController.WeaponsEnabled = true;

        GamePause.RequestResume<PlayerRespawn>();
        // Time.timeScale = 1.0f;
        // mainMixer.SetFloat(PauseMenu.SFXWithPauseVolume, 0.0f);
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

    private void OnPlayerDied()
    {
        GamePause.RequestPause<PlayerRespawn>();
        // Time.timeScale = 0.0f;
        // mainMixer.SetFloat(PauseMenu.SFXWithPauseVolume, -80.0f);
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
            Debug.LogError("Player damage receiver is required for player respawn");
            Destroy(this);
        }

        if (stamina == null)
        {
            Debug.LogError("Player stamina is required for player respawn");
            Destroy(this);
        }

        if (weaponController == null)
        {
            Debug.LogError("Player weapon controller is required for player respawn");
            Destroy(this);
        }

        if (mainMixer == null)
        {
            Debug.LogError("Main mixer is required for player respawn");
            Destroy(this);
        }
    }
}