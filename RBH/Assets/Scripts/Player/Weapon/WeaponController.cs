using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private const int InvalidWeaponIndex = -1;

    [SerializeField]
    private DayNightPhase weaponDisablePhase;

    [SerializeField]
    private List<PlayerProjectileWeapon> playerProjectileWeapons = new();

    public bool WeaponsEnabled { get; set; } = true;

    private int currentWeaponIndex = InvalidWeaponIndex;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(weaponDisablePhase != null, $"{nameof(weaponDisablePhase)} is required for {nameof(WeaponController)}", this);

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;

        TrySelectFirstWeapon();
    }

    private void TrySelectFirstWeapon()
    {
        if (playerProjectileWeapons.Count == 0)
        {
            return;
        }

        currentWeaponIndex = 0;
        playerProjectileWeapons[currentWeaponIndex].Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.isPaused)
        {
            return;
        }

        if (!WeaponsEnabled)
        {
            return;
        }

        HandleWeaponSwitch();
        HandleWeaponInput();
    }

    private void OnDestroy()
    {
        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }
    }

    private void HandleWeaponSwitch()
    {
        if (playerProjectileWeapons.Count <= 1)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Alpha1)
            && !Input.GetKeyDown(KeyCode.Alpha2))
        {
            return;
        }

        var nextWeaponIndex = currentWeaponIndex + 1;

        if (nextWeaponIndex == playerProjectileWeapons.Count)
        {
            nextWeaponIndex = 0;
        }

        playerProjectileWeapons[currentWeaponIndex].Deselect();

        currentWeaponIndex = nextWeaponIndex;

        playerProjectileWeapons[currentWeaponIndex].Select();

        Debug.Log($"Next weapon index: {currentWeaponIndex}, next weapon name: {playerProjectileWeapons[currentWeaponIndex].GetType().Name}", this);
    }

    private void HandleWeaponInput()
    {
        if (currentWeaponIndex == InvalidWeaponIndex)
        {
            return;
        }

        var currentWeapon = playerProjectileWeapons[currentWeaponIndex];

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentWeapon.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.TryReload();
        }
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase == weaponDisablePhase)
        {
            WeaponsEnabled = false;
        }
    }
}
