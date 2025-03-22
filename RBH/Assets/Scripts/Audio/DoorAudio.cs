using System;
using UnityEngine;

public class DoorAudio : MonoBehaviour
{
    [SerializeField] private AudioSource doorAudioSource;

    [SerializeField] private AudioClip doorOpenClip;

    [SerializeField] private AudioClip doorCloseClip;

    [SerializeField] private AudioClip doorShakeClip;

    private void Start()
    {
        Debug.Assert(doorAudioSource != null, $"{nameof(doorAudioSource)} is required for {nameof(DoorAudio)}",
            this);
        Debug.Assert(doorOpenClip != null, $"{nameof(doorOpenClip)} is required for {nameof(DoorAudio)}",
            this);
        Debug.Assert(doorCloseClip != null, $"{nameof(doorCloseClip)} is required for {nameof(DoorAudio)}",
            this);
        // Debug.Assert(doorShakeClip != null, $"{nameof(doorShakeClip)} is required for {nameof(DoorAudio)}",
        //     this);
    }

    public void PlayOpenAudio() => PlayClip(doorOpenClip);

    public void PlayCloseAudio() => PlayClip(doorCloseClip);

    public void StartShakeAudio() => PlayClip(doorShakeClip, true);

    public void StopShakeAudio()
    {
        if (!doorAudioSource.clip != doorShakeClip)
        {
            return;
        }

        doorAudioSource.Stop();
        doorAudioSource.clip = null;
    }

    private void PlayClip(AudioClip clip, bool loop = false)
    {
        if (doorAudioSource.isPlaying)
        {
            doorAudioSource.Stop();
            // primaryAudioSource.clip = null;
        }

        doorAudioSource.clip = clip;
        doorAudioSource.loop = loop;
        doorAudioSource.Play();
    }
}