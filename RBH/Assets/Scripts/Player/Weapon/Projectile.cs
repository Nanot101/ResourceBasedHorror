using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 4f;
    [SerializeField] private float projectileSpeed = 2f;

    private Collider2D playerCollider;

    public void InitializeProjectile(float _projectileSpeedMultiplier, Collider2D _playerCollider)
    {
        projectileSpeed *= _projectileSpeedMultiplier;
        playerCollider = _playerCollider;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        if (playerCollider == null)
        {
            Debug.LogError("Player collider is null, missing projectile Initialization");
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
        EnemyStun enemy = other.GetComponent<EnemyStun>();
        if (enemy != null)
        {
            enemy.TryStun();
        }
        Destroy(gameObject);

    }
}
