using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerParticle : MonoBehaviour
{
    public int damage = 10;
    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent<PlayerDamageReceiver>(out var healthSystem))
        {
            healthSystem.TryDealDamage(damage);
        }
    }
}
