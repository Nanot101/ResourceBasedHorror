using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 4f;
    [SerializeField] private float projectileSpeed = 2f;

    [SerializeField]
    private DayNightPhase projectileDestroyPhase;

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
            return;
        if (other.TryGetComponent<EnemyStun>(out var enemy))
        {
            enemy.TryStun();
        }
        Destroy(gameObject);

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
