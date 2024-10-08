using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class CutSceneController : MonoBehaviour
{
    public VideoClip videoClip;
    public string nextSceneName;

    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        SetUpVideoPlayer();
    }

    void SetUpVideoPlayer()
    {
        videoPlayer.clip = videoClip;
        videoPlayer.playOnAwake = true;
        videoPlayer.isLooping = false;
        videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());

        videoPlayer.loopPointReached += EndReached;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SkipCutScene();
        }
    }

    void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void SkipCutScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
