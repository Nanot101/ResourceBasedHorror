using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudio : MonoBehaviour
{
    [SerializeField] AudioSource uiClickAudioSource;
    [SerializeField] AudioClip uiClickAudioClip;

    private void Start()
    {
        uiClickAudioSource.ignoreListenerPause = true;
    }

    public void PlayAudioSFX()
    {
        uiClickAudioSource.PlayOneShot(uiClickAudioClip);
    }
}
