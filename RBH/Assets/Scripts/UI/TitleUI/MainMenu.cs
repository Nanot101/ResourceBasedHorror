using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // public AudioMixer mainMixer;
    public CanvasGroup mainCanvasGroup; // For fading out the main panel with buttons
    public CanvasGroup blackCanvasGroup; // For fading in the black screen
    public float fadeDuration = 1.0f;
    public static bool isMale;

    private void Start()
    {
        blackCanvasGroup.alpha = 0;
        mainCanvasGroup.alpha = 1;
    }

    public void SetCharacter(bool male)
    {
        isMale = male;
    }

    public void SelectCharacter(PlayerVisual characterVisual)
    {
        var plyrSelectMngr = FindFirstObjectByType<PlyrSelectMngr>();

        if (plyrSelectMngr != null)
        {
            plyrSelectMngr.SelectedPlayerVisual = characterVisual;
        }

        PlayIntro();
    }

    public void PlayIntro()
    {
        StartCoroutine(FadeOutAndStartGame());
    }

    private IEnumerator FadeOutAndStartGame()
    {
        float elapsedTime = 0f;
        // Fade out the main panel and fade in the black screen
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeAmount = elapsedTime / fadeDuration;
            // Fade out
            mainCanvasGroup.alpha = 1 - fadeAmount;
            // Fade in
            blackCanvasGroup.alpha = fadeAmount;
            yield return null;
        }

        // Ensure full fade at the end
        mainCanvasGroup.alpha = 0;
        blackCanvasGroup.alpha = 1;
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void CharSelect()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    // public void SetVolumeMusic(float sliderValue)
    // {
    //     mainMixer.SetFloat("Music", sliderValue);
    // }

    // public void SetVolumeSFX(float sliderValue)
    // {
    //     mainMixer.SetFloat("SFX", sliderValue);
    // }
}