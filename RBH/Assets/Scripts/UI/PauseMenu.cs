using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public const string MusicWithPauseVolume = "MusicWithPauseVolume";
    
    public const string SFXWithPauseVolume = "SFXWithPauseVolume";

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;

    public AudioMixer mainMixer;

    private static bool IsPauseMenuActive => GamePause.IsPauseRequested<PauseMenu>();

    private void Start()
    {
        DisablePauseVolume();
    }

    private void OnDestroy()
    {
        ResumeGame();
    }

   private void Update()
    {
        if (GamePause.IsPaused && !IsPauseMenuActive)
        {
            // The game is paused, but not by us, so our pause request is not processed.
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (IsPauseMenuActive)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        EnablePauseVolume();

        GamePause.RequestPause<PauseMenu>();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        Time.timeScale = 1f;

        DisablePauseVolume();

        GamePause.RequestResume<PauseMenu>();
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    //If we want to keep playing the audio when the game is paused like a menu music, we can use use audioSource.ignoreListenerPause = true;
    // public void PauseAudio()
    // {
    //     AudioListener.pause = true;
    //     menuMusic.Play();
    // }
    //
    // public void ResumeAudio()
    // {
    //     menuMusic.Pause();
    //     AudioListener.pause = false;
    // }

    private void EnablePauseVolume()
    {
        mainMixer.SetFloat(MusicWithPauseVolume, 20 * Mathf.Log10(0.30f));
        mainMixer.SetFloat(SFXWithPauseVolume, -80.0f);
    }

    private void DisablePauseVolume()
    {
        mainMixer.SetFloat(MusicWithPauseVolume, 0.0f);
        mainMixer.SetFloat(SFXWithPauseVolume, 0.0f);
    }
}