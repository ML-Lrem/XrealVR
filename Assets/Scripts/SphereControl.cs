using UnityEngine.Video;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class SphereControl : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Vector3 initialRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    private float gyroInterval = 0.01f; // 读取陀螺仪数据的时间间隔为0.05秒
    private float gyroTimer = 0f; // 陀螺仪计时器

    // Nreal Driver
    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    static extern int StartConnection();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    static extern int StopConnection();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr GetQuaternion();

    [DllImport("AirAPI_Windows", CallingConvention = CallingConvention.Cdecl)]
    static extern IntPtr GetEuler();

    IntPtr EulerPtr = IntPtr.Zero;
    IntPtr initEulerPtr = IntPtr.Zero;

    bool isConnection = false;
    float[] EulerArray = { 0, 0, 0 };
    float[] initEulerArray = { 0, 0, 0 };

    bool isIMUInit = false;
    bool isSphereReset = false;

    void Start()
    {
        // Move by Mouse
        transform.localEulerAngles = initialRotation;
        xRotation = initialRotation[0];
        yRotation = initialRotation[1];
    }

    void Update()
    {
        // Move by Mouse
        if (videoPlayer.url != "" && videoPlayer.isPlaying)
        {
            if (!isConnection)
            {
                // Start the connection
                var res = StartConnection();
                if (res == 1)
                {
                    Debug.Log("Connection started");
                    isConnection = true;
                }
                else
                {
                    Debug.Log("Connection failed");
                }
            }

            // 强制连接
            if (Input.GetKeyDown(KeyCode.C))
            {
                // Start the connection
                var res = StartConnection();
                if (res == 1)
                {
                    Debug.Log("Connection started");
                    isIMUInit = false;
                    isConnection = true;

                }
                else
                {
                    Debug.Log("Connection failed");
                }
            }
            // 更新陀螺仪计时器
            gyroTimer += Time.deltaTime;

            if (gyroTimer >= 7)      // init IMU Times = 3
            {
                Debug.Log("Init IMU");
                initEulerPtr = GetEuler();
                Marshal.Copy(initEulerPtr, initEulerArray, 0, 3);
                Debug.Log("init:" + initEulerArray[1] + ";" + initEulerArray[2]);
                isIMUInit = true;
                gyroTimer = 0f;
            }

            // Get array data from memory
            if (gyroTimer >= gyroInterval && isIMUInit)
            {
                if (isSphereReset)
                {
                    Debug.Log("CameraReset");
                    isSphereReset = false;
                    initEulerPtr = GetEuler();
                    Marshal.Copy(initEulerPtr, initEulerArray, 0, 3);
                }
                else
                {
                    EulerPtr = GetEuler();
                    Marshal.Copy(EulerPtr, EulerArray, 0, 3);
                    yRotation = initialRotation[1] + (EulerArray[2] - initEulerArray[2]);
                    Debug.Log("EulerArray[2]:" + EulerArray[2] + "initEulerArray[2]:" + initEulerArray[2]);
                    yRotation = Mathf.Clamp(yRotation, initialRotation[1] - 50.0f, initialRotation[1] + 50.0f);
                    Debug.Log("yRotation:" + yRotation);
                    // Rotation
                    transform.localRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
                }
                gyroTimer = 0f;
            }

            // Reset Shpere
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isSphereReset = true;
                xRotation = initialRotation[0];
                yRotation = initialRotation[1];
            }
        }
    }
    private void OnApplicationQuit()
    {
        Debug.Log("Stop Nreal");
        StopConnection();
    }
}
