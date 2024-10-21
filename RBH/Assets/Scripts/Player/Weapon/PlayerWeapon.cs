using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] private float cooldown = 6f;

    [SerializeField]
    private DayNightPhase weaponDisablePhase;

    public bool canShoot = true;
    private float cooldownTimer;

    private void Start()
    {
        if (playerCollider == null)
            playerCollider = GetComponentInParent<Collider2D>();

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy() => DayNightSystem.Instance.OnPhaseChanged -= OnDayNightPhaseChanged;

    private void Update()
    {
        if (PauseMenu.isPaused)
            return;
        if (!canShoot)
            return;
        if (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
            cooldownTimer = 0;
        }
    }
    private void Shoot()
    {
        Projectile instantiatedProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        instantiatedProjectile.InitializeProjectile(playerCollider);
    }

    public void GetCooldown(out float currentCooldown, out float maxCooldown)
    {
        currentCooldown = cooldownTimer;
        maxCooldown = cooldown;
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase == weaponDisablePhase)
        {
            canShoot = false;
        }
    }
}
