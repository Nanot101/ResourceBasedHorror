using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Image sliderBackground;

    private PlayerStamina playerStamina;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(slider != null);
        Debug.Assert(sliderBackground != null);

        PlayerSpawnManager.Instance.OnPlayerSpawn += OnPlayerSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStamina == null)
        {
            return;
        }

        slider.value = playerStamina.CurrentStaminaNormalized;

        if (playerStamina.HasStamina)
        {
            sliderBackground.color = Color.white;

            return;
        }

        sliderBackground.color = Color.red;
    }

    private void OnDestroy()
    {
        PlayerSpawnManager.Instance.OnPlayerSpawn -= OnPlayerSpawn;
    }

    private void OnPlayerSpawn(object sender, OnPlayerSpawnArgs args)
    {
        var player = args.SpawnedPlayer.transform;

        playerStamina = player.GetComponent<PlayerStamina>();
    }
}
