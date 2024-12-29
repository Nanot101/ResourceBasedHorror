using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private const string MusicVolume = "MusicVolume";
    private const string MusicWithPauseVolume = "MusicWithPauseVolume";

    private const string SFXVolume = "SFXVolume";
    private const string SFXWithPauseVolume = "SFXWithPauseVolume";

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionsMenu;

    public AudioMixer mainMixer;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private static bool IsPauseMenuActive => GamePause.IsPauseRequested<PauseMenu>();

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(MusicVolume, .5f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFXVolume, 0.8f);

        DisablePauseVolume();
    }

    private void OnEnable()
    {
        musicSlider.onValueChanged.AddListener(SetVolumeMusic);
        sfxSlider.onValueChanged.AddListener(SetVolumeSFX);
    }

    private void OnDisable()
    {
        musicSlider.onValueChanged.RemoveListener(SetVolumeMusic);
        sfxSlider.onValueChanged.RemoveListener(SetVolumeSFX);
    }

    private void OnDestroy()
    {
        ResumeGame();
    }

    void Update()
    {
        if (GamePause.IsPaused && !IsPauseMenuActive)
        {
            // The game is paused, but not by us, so our pause request is not processed.
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPauseMenuActive)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
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

    public void SetVolumeMusic(float sliderValue)
    {
        PlayerPrefs.SetFloat(MusicVolume, sliderValue);

        var dB = SliderValueToDB(sliderValue);

        mainMixer.SetFloat(MusicVolume, dB);
    }

    public void SetVolumeSFX(float sliderValue)
    {
        PlayerPrefs.SetFloat(SFXVolume, sliderValue);

        var dB = SliderValueToDB(sliderValue);

        mainMixer.SetFloat(SFXVolume, dB);
    }

    private static float SliderValueToDB(float sliderValue)
    {
        var dB = 20 * Mathf.Log10(sliderValue);

        if (sliderValue <= 0)
        {
            dB = -80;
        }

        return dB;
    }
}