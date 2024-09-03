using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEnergyBarUI : MonoBehaviour
{
    [SerializeField] private PlayerWeapon playerWeapon;
    [SerializeField] private Image weaponEnergyBar;

    private void Awake()
    {
        if (playerWeapon == null)
        {
            playerWeapon = FindObjectOfType<PlayerWeapon>();
            if (playerWeapon == null)
            {
                Debug.LogError("PlayerWeapon reference is null and not found within the scene");
                return;
            }
        }
    }

    private void Update()
    {
        playerWeapon.GetCooldown(out float currentCooldown, out float maxCooldown);
        weaponEnergyBar.fillAmount = currentCooldown / maxCooldown;
    }

}
