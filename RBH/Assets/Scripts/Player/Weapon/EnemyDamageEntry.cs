using System;
using UnityEngine;

[Serializable]
public struct EnemyDamageEntry : IEquatable<EnemyDamageEntry>, IEquatable<EnemySO>
{
    [field: SerializeField]
    public EnemySO Enemy { get; set; }

    [field: SerializeField]
    public float Damage { get; set; }

    [field: SerializeField]
    public bool CanStun { get; set; }

    public readonly bool Equals(EnemyDamageEntry other)
    {
        if (other.Enemy == null
            || !other.Enemy.Equals(Enemy))
        {
            return false;
        }

        if (!other.Damage.Equals(Damage))
        {
            return false;
        }

        if (!other.CanStun.Equals(CanStun))
        {
            return false;
        }

        return true;
    }

    public readonly bool Equals(EnemySO other) => other.Equals(Enemy);
}
