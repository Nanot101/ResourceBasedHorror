using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Image sliderBackground;

    [SerializeField]
    private PlayerStamina playerStamina;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(slider != null);
        Debug.Assert(sliderBackground != null);

        PlayerHealthSystem.onPlayerDied += OnPlayerDied;
    }

    private void OnDestroy() => PlayerHealthSystem.onPlayerDied -= OnPlayerDied;

    // Update is called once per frame
    void Update()
    {
        slider.value = playerStamina.CurrentStaminaNormalized;

        if (playerStamina.HasStamina)
        {
            sliderBackground.color = Color.white;

            return;
        }

        sliderBackground.color = Color.red;
    }

    private void OnPlayerDied() => enabled = false;

    public void OnPlayerRespawned() => enabled = true;
}
