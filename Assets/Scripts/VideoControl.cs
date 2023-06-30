using UnityEngine.Video;
using UnityEngine;

public class VideoControl : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public int speedRate = 30;

    // Start is called before the first frame update
    void Start()
    {
        // Prepare
        videoPlayer.Prepare();
    }

    // Update is called once per frame
    void Update()
    {
        // �������
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            videoPlayer.frame += (long)(videoPlayer.frameRate * speedRate);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            videoPlayer.frame -= (long)(videoPlayer.frameRate * speedRate);
        }

        // ����ESC��ʱֹͣ��Ƶ���Ų��ر���Ƶ��Ⱦ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Cursor.lockState = CursorLockMode.Confined;
            videoPlayer.Stop();
            videoPlayer.targetCameraAlpha = 0;
        }
    }
}
