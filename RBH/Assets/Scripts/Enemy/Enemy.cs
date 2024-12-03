using UnityEngine;

[RequireComponent(typeof(EnemyHealth), typeof(EnemyDropSystem))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth health;

    [SerializeField]
    private EnemyDropSystem dropSystem;

    [field: SerializeField]
    public EnemySO Attributes { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(Attributes != null, $"{nameof(Attributes)} are required for {nameof(Enemy)}", this);
        Debug.Assert(health != null, $"{nameof(health)} is required for {nameof(Enemy)}", this);
        Debug.Assert(dropSystem != null, $"{nameof(dropSystem)} is required for {nameof(Enemy)}", this);

        health.SetHealth(Attributes.HealthPoints);
        dropSystem.SetDrops(Attributes.Drops);
    }
}
