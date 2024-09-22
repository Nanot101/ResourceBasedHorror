using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyEnemySO", menuName = "Enemy SO")]
public class EnemySO : ScriptableObject
{
    [field: SerializeField]
    public float HealthPoints { get; set; }

    [Tooltip("Initial speed of enemy")]
    [field: SerializeField]
    public float WalkSpeed { get; set; }

    [Tooltip("Speed of enemy when he sees player")]
    [field: SerializeField]
    public float RunSpeed { get; set; }

    [field: SerializeField]
    public int OnNight { get; set; }

    [field: SerializeField]
    public List<EnemyDropSO> Drops { get; set; }
}
