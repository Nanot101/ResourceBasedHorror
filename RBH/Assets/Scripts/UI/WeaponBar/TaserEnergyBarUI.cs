using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaserEnergyBarUI : MonoBehaviour
{
    [SerializeField]
    private TaserWeapon taserWeapon;

    [SerializeField]
    private Image taserEnergyBar;

    [SerializeField]
    private GameObject weaponIcon;

    private void Start()
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

        taserWeapon.OnTaserSelected += OnTaserSelected;
        taserWeapon.OnTaserDeselected += OnTaserDeselected;

        // weaponIcon.SetActive(false);
        //
        // gameObject.SetActive(false);
    }

    private void Update()
    {
        taserWeapon.GetCooldown(out float currentCooldown, out float maxCooldown);

        var cooldownPercent = currentCooldown / maxCooldown;

        taserEnergyBar.fillAmount = cooldownPercent;

        ShowWeaponIconBasedOnCooldown(cooldownPercent);
    }

    private void OnDestroy()
    {
        if (taserWeapon != null) 
        {
            taserWeapon.OnTaserSelected -= OnTaserSelected;
            taserWeapon.OnTaserDeselected -= OnTaserDeselected;
        }
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

    private void OnTaserSelected(object sender, EventArgs e) => gameObject.SetActive(true);

    private void OnTaserDeselected(object sender, EventArgs e) => gameObject.SetActive(false);
}
