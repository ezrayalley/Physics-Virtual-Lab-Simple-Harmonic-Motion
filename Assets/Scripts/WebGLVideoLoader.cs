using UnityEngine;
using UnityEngine.Video;

public class WebGLVideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool hasPlayed = false;

    void Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "SHMFullExplanation1.mp4");

#if UNITY_WEBGL && !UNITY_EDITOR
        videoPlayer.url = path;  // WebGL uses URL directly
#else
        videoPlayer.url = "file://" + path;  // Editor/PC needs this
#endif
    }
    void Update()
    {
        // Detect mouse click OR screen touch
        if (!hasPlayed && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            videoPlayer.Play();
            hasPlayed = true;
        }
    }
}