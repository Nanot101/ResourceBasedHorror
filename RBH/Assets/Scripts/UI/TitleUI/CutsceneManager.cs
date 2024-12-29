using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public GameObject maleVideoPlayer; // Assign the male video player GameObject
    public GameObject femaleVideoPlayer; // Assign the female video player GameObject
    public GameObject nextScene;
    

    private void Start()
    {
        // Disable both video players initially
        maleVideoPlayer.SetActive(false);
        femaleVideoPlayer.SetActive(false);

        if (nextScene != null)
        {
            nextScene.SetActive(false);
        }

        // Check the isMale boolean and activate the appropriate video player
        if (MainMenu.isMale)
        {
            maleVideoPlayer.SetActive(true);
            PlayVideo(maleVideoPlayer);
        }
        else
        {
            femaleVideoPlayer.SetActive(true);
            PlayVideo(femaleVideoPlayer);
        }
    }

    private void PlayVideo(GameObject videoPlayer)
    {
        VideoPlayer vp = videoPlayer.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.loopPointReached += OnVideoFinished; // Subscribe to the video end event
            vp.Play();
        }
        else
        {
            Debug.LogError("No VideoPlayer component found on " + videoPlayer.name);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(EnableNextSceneAfterDelay(2f)); // Wait 2 seconds before enabling nextScene
    }

    private System.Collections.IEnumerator EnableNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (nextScene != null)
        {
            nextScene.SetActive(true);
        }
        else
        {
            Debug.LogError("nextScene GameObject is not assigned!");
        }
    }
}
