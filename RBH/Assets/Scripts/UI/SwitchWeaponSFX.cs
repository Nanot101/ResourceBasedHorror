using UnityEngine;

public class PlaySoundOnKeyPress : MonoBehaviour
{
    public AudioClip SwitchWeaponClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (GamePause.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 key
        {
            if (SwitchWeaponClip != null)
            {
                audioSource.PlayOneShot(SwitchWeaponClip);
            }
            else
            {
                Debug.LogWarning("No audio clip assigned!");
            }
        }
    }
}
