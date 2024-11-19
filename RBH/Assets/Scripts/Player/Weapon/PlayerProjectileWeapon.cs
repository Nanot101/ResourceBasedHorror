using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Marker class for all player projectile weapons.
/// </summary>
public abstract class PlayerProjectileWeapon : MonoBehaviour
{
    public abstract bool TryShoot();

    public abstract bool TryReload();

    public abstract void Select();

    public abstract void Deselect();
}
