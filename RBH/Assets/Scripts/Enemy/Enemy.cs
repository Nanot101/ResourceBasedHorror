using UnityEngine;

[RequireComponent(typeof(EnemyHealth), typeof(EnemyDropSystem))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth health;

    [SerializeField]
    private EnemyDropSystem dropSystem;

    [SerializeField]
    private EnemySO attributes;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(attributes != null, $"{nameof(attributes)} are required for {nameof(Enemy)}", this);
        Debug.Assert(health != null, $"{nameof(health)} is required for {nameof(Enemy)}", this);
        Debug.Assert(dropSystem != null, $"{nameof(dropSystem)} is required for {nameof(Enemy)}", this);

        health.SetHealth(attributes.HealthPoints);
        dropSystem.SetDrops(attributes.Drops);
    }
}
