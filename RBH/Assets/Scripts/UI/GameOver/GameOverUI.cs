using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Button respawnButton;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(respawnButton != null, "Respawn button must be set");
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void AddActionToRespawnButton(UnityAction action)
    {
        respawnButton.onClick.AddListener(action);
    }

    public void RemoveActionFromRespawnButton(UnityAction action)
    {
        respawnButton.onClick.RemoveListener(action);
    }
}
