using UnityEngine;

public class InventoryOpenCloseAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private AudioClip openClip;
    
    [SerializeField] private AudioClip closeClip;

    private void Start()
    {
        Debug.Assert(audioSource, $"{nameof(audioSource)} is required for {nameof(InventoryOpenCloseAudio)}", this);
        Debug.Assert(openClip, $"{nameof(openClip)} is required for {nameof(InventoryOpenCloseAudio)}", this);
        Debug.Assert(closeClip, $"{nameof(closeClip)} is required for {nameof(InventoryOpenCloseAudio)}", this);
    }

    public void PlayOpen() => PlayClip(openClip);

    public void PlayClose() => PlayClip(closeClip);

    private void PlayClip(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            // audioSource.clip = null;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }
}