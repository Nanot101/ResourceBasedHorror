using UnityEngine;
using UnityEngine.SceneManagement;

public class PlyrSelectMngr : MonoBehaviour
{
    [SerializeField]
    private int mainLevelIndex = 3;

    public PlayerVisual SelectedPlayerVisual { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != mainLevelIndex
            || SelectedPlayerVisual == null)
        {
            return;
        }

        var playerVisualSelector = FindFirstObjectByType<PlayerVisualSetter>();

        if (playerVisualSelector != null)
        {
            playerVisualSelector.SetVisual(SelectedPlayerVisual);
        }
    }
}
