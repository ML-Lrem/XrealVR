using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using SFB;
using System;

public class OpenVideo : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Text videoPathText;

    private string filePath;

    private void OnGUI()
    {
        // ����GUIStyle����
        GUIStyle style = new GUIStyle(GUI.skin.button);
        // ���������СΪ20
        style.fontSize = 16;
        if (GUILayout.Button("Select File", style,GUILayout.Width(100), GUILayout.Height(40)))
        {
            string extensions = "mp4";
            string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
            if (!string.IsNullOrEmpty(path[0])) // ��ʾ�ļ��Ի���
            {
                filePath = path[0];
                try
                {
                    // ����VideoPlayer��url
                    videoPlayer.url = filePath;
                    videoPlayer.Stop();
                    // ��ʾѡ����ļ�·��
                    videoPathText.text = "Video Path: " + filePath;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError("Error: " + ex.Message);
                }
            }
        }

        if (GUILayout.Button("Play", style, GUILayout.Width(100), GUILayout.Height(40)))
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                // UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                videoPlayer.Play();
                UnityEngine.Debug.Log("Selected file: " + filePath);
            }
        }
    }
}
