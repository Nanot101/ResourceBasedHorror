using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaserEnergyBarUI : MonoBehaviour
{
    [SerializeField] private TaserWeapon taserWeapon;
    [SerializeField] private Image taserEnergyBar;
    [SerializeField] private GameObject weaponIcon;

    private void Awake()
    {
        if (taserWeapon == null)
        {
            taserWeapon = FindObjectOfType<TaserWeapon>();
            if (taserWeapon == null)
            {
                Debug.LogError("TaserWeapon reference is null and not found within the scene");
                return;
            }
        }
    }

    private void Start()
    {
        weaponIcon.SetActive(false);
    }

    private void Update()
    {
        taserWeapon.GetCooldown(out float currentCooldown, out float maxCooldown);

        var cooldownPercent = currentCooldown / maxCooldown;

        taserEnergyBar.fillAmount = cooldownPercent;

        ShowWeaponIconBasedOnCooldown(cooldownPercent);
    }

    private void ShowWeaponIconBasedOnCooldown(float cooldownPercent)
    {
        if (cooldownPercent < 1.0f)
        {
            weaponIcon.SetActive(false);
            return;
        }

        weaponIcon.SetActive(true);
    }
}
