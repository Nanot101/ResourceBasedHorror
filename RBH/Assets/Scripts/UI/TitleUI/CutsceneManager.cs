using UnityEngine;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public GameObject maleVideoPlayer; // Assign the male video player GameObject
    public GameObject femaleVideoPlayer; // Assign the female video player GameObject
    public GameObject nextScene;

    public string maleVideoFileName;
    public string femaleVideoFileName;

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
            PlayVideo(maleVideoPlayer, maleVideoFileName);
        }
        else
        {
            femaleVideoPlayer.SetActive(true);
            PlayVideo(femaleVideoPlayer, femaleVideoFileName);
        }
    }

    private void PlayVideo(GameObject videoPlayer, string videoFileName)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(videoFileName), "Please provide valid file name", this);

        //this assert should be a regex
        Debug.Assert(videoFileName.Contains('.'), "Please provide valid file name with extension",
            this);

        VideoPlayer vp = videoPlayer.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
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