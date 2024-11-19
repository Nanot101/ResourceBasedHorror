using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaserWeapon : PlayerProjectileWeapon
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] Projectile projectilePrefab;
    [SerializeField] private float cooldown = 6f;

    private float cooldownTimer;

    private void Start()
    {
        if (playerCollider == null)
            playerCollider = GetComponentInParent<Collider2D>();
    }

    private void Update()
    {
        if (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    public override bool TryShoot()
    {
        if (cooldownTimer < cooldown)
        {
            return false;
        }

        Shoot();

        cooldownTimer = 0;

        return true;
    }

    public override bool TryReload()
    {
        // NOOP
        return false;
    }

    public override void Select()
    {
        // NOOP
        return;
    }

    public override void Deselect()
    {
        // NOOP
        return;
    }

    public void GetCooldown(out float currentCooldown, out float maxCooldown)
    {
        currentCooldown = cooldownTimer;
        maxCooldown = cooldown;
    }

    private void Shoot()
    {
        Projectile instantiatedProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        instantiatedProjectile.InitializeProjectile(playerCollider);
    }
}
