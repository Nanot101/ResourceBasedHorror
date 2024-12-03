using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 4f;

    [SerializeField]
    private float projectileSpeed = 2f;

    [SerializeField]
    private DayNightPhase projectileDestroyPhase;

    [SerializeField]
    private List<EnemyDamageEntry> enemyDamageTable = new();

    private Collider2D playerCollider;

    public void InitializeProjectile(Collider2D _playerCollider)
    {
        playerCollider = _playerCollider;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        if (playerCollider == null)
        {
            Debug.LogError("Player collider is null, missing projectile Initialization");
        }

        DayNightSystem.Instance.OnPhaseChanged += OnDayNightPhaseChanged;
    }

    private void OnDestroy()
    {
        if (DayNightSystem.TryGetInstance(out var dayNightSys))
        {
            dayNightSys.OnPhaseChanged -= OnDayNightPhaseChanged;
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.up * projectileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == playerCollider)
        {
            return;
        }

        TryDealDamage(other);

        Destroy(gameObject);
    }

    private void TryDealDamage(Collider2D other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        var enemyDamageEntry = enemyDamageTable.SingleOrDefault(x => x.Equals(enemy.Attributes));

        if (enemyDamageEntry.Equals((EnemyDamageEntry)default))
        {
            return;
        }

        if (enemyDamageEntry.CanStun
            && enemy.TryGetComponent<EnemyStun>(out var enemyStun))
        {
            enemyStun.TryStun();
        }

        if (enemyDamageEntry.Damage > 0.0f
            && enemy.TryGetComponent<EnemyHealth>(out var enemyHealth))
        {
            enemyHealth.DecreaseHealth(enemyDamageEntry.Damage);
        }
    }

    private void OnDayNightPhaseChanged(object sender, DayNightSystemEventArgs args)
    {
        var currentPhase = args.CurrentPhase;

        if (currentPhase == projectileDestroyPhase)
        {
            Destroy(gameObject);
        }
    }
}
