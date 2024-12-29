using System;
using UnityEngine;

public class TaserWeapon : PlayerProjectileWeapon
{
    [SerializeField] private GunAudio gunAudio;

    [SerializeField] private Collider2D playerCollider;

    [SerializeField] Projectile projectilePrefab;

    [SerializeField] private float cooldown = 6f;

    [field: SerializeField] public AudioClip FireAudioClip { get; private set; }

    private float cooldownTimer;

    public event EventHandler OnTaserSelected;

    public event EventHandler OnTaserDeselected;

    private void Start()
    {
        if (playerCollider == null)
            playerCollider = GetComponentInParent<Collider2D>();

        Debug.Assert(gunAudio != null, $"{nameof(gunAudio)} is required for {nameof(TaserWeapon)}", this);
        Debug.Assert(FireAudioClip != null, $"{nameof(FireAudioClip)} is required for {nameof(TaserWeapon)}", this);
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

        SpawnProjectile();
        PlayAudio();

        cooldownTimer = 0;

        return true;
    }

    public override bool TryReload()
    {
        // NOOP
        return false;
    }

    public override void Select() => OnTaserSelected?.Invoke(this, EventArgs.Empty);

    public override void Deselect() => OnTaserDeselected?.Invoke(this, EventArgs.Empty);

    public void GetCooldown(out float currentCooldown, out float maxCooldown)
    {
        currentCooldown = cooldownTimer;
        maxCooldown = cooldown;
    }

    private void SpawnProjectile()
    {
        Projectile instantiatedProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        instantiatedProjectile.InitializeProjectile(playerCollider);
    }

    private void PlayAudio() => gunAudio.PlayPrimaryAudio(FireAudioClip);
}