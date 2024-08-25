using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Button respawnButton;

    [SerializeField]
    private PlayerRespawn playerRespawn;

    // Start is called before the first frame update
    void Start()
    {
        if (respawnButton == null)
        {
            Debug.LogError("Respawn button is required for game over UI");
            Destroy(this);
        }

        if (playerRespawn == null)
        {
            Debug.LogError("Player respawn is required for game over UI");
            Destroy(this);
        }

        respawnButton.onClick.AddListener(OnRespawnButtonClick);

        PlayerHealthSystem.onPlayerDied += OnPlayerDied;

        gameObject.SetActive(false);
    }

    private void OnDestroy() => PlayerHealthSystem.onPlayerDied -= OnPlayerDied;

    public void AddActionToRespawnButton(UnityAction action)
    {
        respawnButton.onClick.AddListener(action);
    }

    public void OnPlayerDied() => gameObject.SetActive(true);

    private void OnRespawnButtonClick()
    {
        gameObject.SetActive(false);

        playerRespawn.RespawnPlayer();
    }
}
