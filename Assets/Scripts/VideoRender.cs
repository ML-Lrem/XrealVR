using UnityEngine;
using UnityEngine.Video;

public class VideoRender : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private bool isPlaying;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        isPlaying = false;
    }

    void Update()
    {
        if (videoPlayer.isPlaying && !isPlaying)
        {
            meshRenderer.enabled = true;
            isPlaying = true;
        }
        else if (!videoPlayer.isPlaying && isPlaying)
        {
            meshRenderer.enabled = false;
            isPlaying = false;
        }
    }

    void LateUpdate()
    {
        if (videoPlayer.isPlaying)
        {
            if (Mathf.Abs(videoPlayer.frameRate - Time.captureFramerate) > 1)
            {
                meshRenderer.enabled = false;
            }
            else
            {
                meshRenderer.enabled = true;
            }
        }
    }
}
