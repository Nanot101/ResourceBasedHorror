using UnityEngine;

public class GunAudio : MonoBehaviour
{
    [SerializeField] private AudioSource primaryAudioSource;

    [SerializeField] private AudioSource secondaryAudioSource;

    public void Start()
    {
        Debug.Assert(primaryAudioSource != null, $"{nameof(primaryAudioSource)} is required for {nameof(GunAudio)}",
            this);
        Debug.Assert(secondaryAudioSource != null, $"{nameof(secondaryAudioSource)} is required for {nameof(GunAudio)}",
            this);
    }

    public void PlayPrimaryAudio(AudioClip clip)
    {
        if (primaryAudioSource.isPlaying)
        {
            primaryAudioSource.Stop();
            // primaryAudioSource.clip = null;
        }

        primaryAudioSource.clip = clip;
        primaryAudioSource.Play();
    }

    public void PlaySecondaryAudio(AudioClip clip)
    {
        if (secondaryAudioSource.isPlaying)
        {
            secondaryAudioSource.Stop();
            // secondaryAudioSource.clip = null;
        }

        secondaryAudioSource.clip = clip;
        secondaryAudioSource.Play();
    }
}