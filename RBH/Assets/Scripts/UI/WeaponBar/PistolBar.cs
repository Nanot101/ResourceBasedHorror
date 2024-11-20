using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PistolBar : MonoBehaviour
{
    [SerializeField]
    private PistolWeapon pistolWeapon;

    [SerializeField]
    private Slider CooldownSlider;

    [SerializeField]
    private TextMeshProUGUI ammoText;

    [SerializeField]
    private string reloadingText = "Reloading";

    private void Start()
    {
        if (pistolWeapon == null)
        {
            pistolWeapon = FindObjectOfType<PistolWeapon>();

            Debug.Assert(pistolWeapon != null, $"{nameof(pistolWeapon)} reference is null and not found within the scene", this);
        }

        Debug.Assert(CooldownSlider != null, $"{nameof(CooldownSlider)} is is required for {nameof(PistolBar)}", this);
        Debug.Assert(ammoText != null, $"{nameof(ammoText)} is is required for {nameof(PistolBar)}", this);

        pistolWeapon.OnPistolSelected += OnPistolSelected;
        pistolWeapon.OnPistolDeselected += OnPistolDeselected;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCooldownBar();
        UpdateAmmoText();
    }

    private void UpdateCooldownBar()
    {
        if (pistolWeapon.CurrentState == PistolWeapon.State.ShootCooldown)
        {
            CooldownSlider.value = GetCooldownPercentage(pistolWeapon.CooldownTimer, pistolWeapon.CooldownBetweenShots);
            return;
        }

        if (pistolWeapon.CurrentState == PistolWeapon.State.ReloadCooldown)
        {
            CooldownSlider.value = GetCooldownPercentage(pistolWeapon.CooldownTimer, pistolWeapon.ReloadCooldown);
            return;
        }

        CooldownSlider.value = 1.0f;

        static float GetCooldownPercentage(float currentCooldown, float maxCooldown) => currentCooldown / maxCooldown;
    }


    private void UpdateAmmoText()
    {
        if (pistolWeapon.CurrentState == PistolWeapon.State.ReloadCooldown)
        {
            SetReloadingText();
            return;
        }

        SetAmmoText();
    }

    private void SetReloadingText()
    {
        ammoText.text = reloadingText;
    }

    private void SetAmmoText()
    {
        var magazineSize = pistolWeapon.MagazineSize;
        var currentBulletsInMagazine = pistolWeapon.CurrentBulletsInMagazine;

        ammoText.text = currentBulletsInMagazine + "/" + magazineSize;
    }

    private void OnDestroy()
    {
        if (pistolWeapon != null)
        {
            pistolWeapon.OnPistolSelected -= OnPistolSelected;
            pistolWeapon.OnPistolDeselected -= OnPistolDeselected;
        }
    }

    private void OnPistolSelected(object sender, EventArgs e) => gameObject.SetActive(true);

    private void OnPistolDeselected(object sender, EventArgs e) => gameObject.SetActive(false);
}
