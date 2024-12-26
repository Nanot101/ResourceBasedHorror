using System;
using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [SerializeField] private AudioSource doorAudioSource;

    [SerializeField] private AudioClip doorOpenClip;

    [SerializeField] private AudioClip doorCloseClip;

    private void Start()
    {
        Debug.Assert(doorAudioSource != null, $"{nameof(doorAudioSource)} is required for {nameof(DoorAudio)}",
            this);
        Debug.Assert(doorOpenClip != null, $"{nameof(doorOpenClip)} is required for {nameof(DoorAudio)}",
            this);
        Debug.Assert(doorCloseClip != null, $"{nameof(doorCloseClip)} is required for {nameof(DoorAudio)}",
            this);
    }

    public void PlayOpenAudio() => PlayClip(doorOpenClip);

    public void PlayCloseAudio() => PlayClip(doorCloseClip);

    private void PlayClip(AudioClip clip)
    {
        if (doorAudioSource.isPlaying)
        {
            doorAudioSource.Stop();
            // primaryAudioSource.clip = null;
        }

        doorAudioSource.clip = clip;
        doorAudioSource.Play();
    }
}