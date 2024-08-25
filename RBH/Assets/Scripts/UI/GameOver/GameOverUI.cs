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
        Debug.Assert(respawnButton != null, "Respawn button is required for game over UI");
        Debug.Assert(playerRespawn != null, "Player respawn is required for game over UI");

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
