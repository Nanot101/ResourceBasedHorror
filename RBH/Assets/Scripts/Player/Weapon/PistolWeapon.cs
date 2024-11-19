using UnityEngine;

public class PistolWeapon : PlayerProjectileWeapon
{
    [SerializeField]
    private Collider2D playerCollider;

    [SerializeField]
    private Projectile projectilePrefab;

    [field: SerializeField]
    public float CooldownBetweenShots { get; private set; } = 1f;

    [field: SerializeField]
    public float ReloadCooldown { get; private set; } = 5f;

    [field: SerializeField]
    public int MagazineSize { get; private set; } = 5;

    public int CurrentBulletsInMagazine { get; private set; } = 0;

    public float CooldownTimer { get; private set; }

    public State CurrentState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        AsserDesignerFields();

        CurrentBulletsInMagazine = MagazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case State.ShootCooldown:
                HandleShootCooldownState();
                break;
            case State.ReloadCooldown:
                HandleReloadCooldownState();
                break;
            default:
                break;
        }
    }

    public override bool TryShoot()
    {
        if (CurrentState != State.Idle)
        {
            return false;
        }

        if (CurrentBulletsInMagazine == 0)
        {
            return false;
        }

        SpawnProjectile();

        CooldownTimer = 0;
        CurrentBulletsInMagazine--;
        CurrentState = State.ShootCooldown;

        Debug.Log($"Pistol fire!, current ammo: {CurrentBulletsInMagazine}", this);

        return true;
    }

    public override bool TryReload()
    {
        if (CurrentState != State.Idle)
        {
            return false;
        }

        if (CurrentBulletsInMagazine == MagazineSize)
        {
            return false;
        }

        CooldownTimer = 0;
        CurrentState = State.ReloadCooldown;

        Debug.Log("Pistol reload!", this);

        return true;
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

    private void SpawnProjectile()
    {
        Projectile instantiatedProjectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        instantiatedProjectile.InitializeProjectile(playerCollider);
    }

    private void HandleShootCooldownState()
    {
        CooldownTimer += Time.deltaTime;

        if (CooldownTimer < CooldownBetweenShots)
        {
            return;
        }

        CurrentState = State.Idle;

        Debug.Log("Shoot cooldown end!", this);

        if (CurrentBulletsInMagazine == 0)
        {
            TryReload();
        }
    }

    private void HandleReloadCooldownState()
    {
        CooldownTimer += Time.deltaTime;

        if (CooldownTimer < ReloadCooldown)
        {
            return;
        }

        CurrentBulletsInMagazine = MagazineSize;
        CurrentState = State.Idle;

        Debug.Log("Reload cooldown end!", this);
    }

    private void AsserDesignerFields()
    {
        Debug.Assert(playerCollider != null, $"{nameof(playerCollider)} is required for {nameof(PistolWeapon)}", this);
        Debug.Assert(projectilePrefab != null, $"{nameof(projectilePrefab)} is required for {nameof(PistolWeapon)}", this);
        Debug.Assert(CooldownBetweenShots > 0.0f, $"{nameof(CooldownBetweenShots)} must be greater than zero in {nameof(PistolWeapon)}", this);
        Debug.Assert(ReloadCooldown > 0.0f, $"{nameof(ReloadCooldown)} must be greater than zero in {nameof(PistolWeapon)}", this);
        Debug.Assert(MagazineSize > 0.0f, $"{nameof(MagazineSize)} must be greater than zero in {nameof(PistolWeapon)}", this);
    }

    public enum State
    {
        Idle,
        ShootCooldown,
        ReloadCooldown
    }
}
