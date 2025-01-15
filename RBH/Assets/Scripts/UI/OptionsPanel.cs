using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsPanel : MonoBehaviour
{
    private const string MusicVolume = "MusicVolume";
    private const string SFXVolume = "SFXVolume";

    private const float DefaultMusicSliderValue = 0.5f;
    private const float DefaultSFXSliderValue = 0.8f;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioMixer mainMixer;

    private void Start()
    {
        RestoreSavedMusicSliderValue();
        RestoreSavedSFXSliderValue();
    }

    public void SetMusicVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(MusicVolume, sliderValue);

        SetMixerVolumeFromSlider(MusicVolume, sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        PlayerPrefs.SetFloat(SFXVolume, sliderValue);

        SetMixerVolumeFromSlider(SFXVolume, sliderValue);
    }

    private void RestoreSavedMusicSliderValue()
    {
        var savedSliderValue = PlayerPrefs.GetFloat(MusicVolume, DefaultMusicSliderValue);

        musicSlider.value = savedSliderValue;

        SetMixerVolumeFromSlider(MusicVolume, savedSliderValue);
    }

    private void RestoreSavedSFXSliderValue()
    {
        var savedSliderValue = PlayerPrefs.GetFloat(SFXVolume, DefaultSFXSliderValue);

        sfxSlider.value = savedSliderValue;

        SetMixerVolumeFromSlider(SFXVolume, savedSliderValue);
    }

    private void SetMixerVolumeFromSlider(string name, float sliderValue)
    {
        var dB = SliderValueToDB(sliderValue);

        mainMixer.SetFloat(name, dB);
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