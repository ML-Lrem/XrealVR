using UnityEngine.Video;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class CameraControl : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    //public int sensitivity = 5;
    public int zoomSpeed = 5;
    public float minFOV = 20.0f;
    public float maxFOV =100.0f;
    public Vector3 initiAngle = new Vector3(0.0f, 0.0f, 0.0f);
    private Quaternion fromRotation;
    private Quaternion toRotation;
    private float lerpSmoothness = 0.1f;
    private Vector3 targetAngle = new Vector3(0.0f, 0.0f, 0.0f);
    private float currentFOV;
    private float gyroInterval = 0.008f; // 读取陀螺仪数据的时间间隔为0.05秒
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

    float[] EulerArray = { 0, 0, 0 };
    float[] initEulerArray = { 0, 0, 0 };
    float eyeDis = 0f;
    float zoomValue = 0f;
    bool isIMUInit = false;
    bool isCameraReset = false;
    bool isConnection = false;

    bool isRightCamera;

    void Start()
    {
        // Move by Mouse
        transform.localEulerAngles = initiAngle;
        fromRotation = transform.rotation;
        toRotation = fromRotation;
        // FOV by UP&Down
        currentFOV = Camera.main.fieldOfView;

        // Ognize Camera Type
        if(initiAngle.y>0)
        {
            Debug.Log("LeftCamera");
            isRightCamera = false;
        }
        else
        {
            Debug.Log("RightCamera");
            isRightCamera = true;
        }
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

            if (gyroTimer >= 7)      // init IMU Times = 7
            {
                Debug.Log("Init IMU");
                initEulerPtr = GetEuler();
                Marshal.Copy(initEulerPtr, initEulerArray, 0, 3);
                Debug.Log("init:" + initEulerArray[1] + ";" + initEulerArray[2]);
                if (initEulerArray[1]!=0)
                {
                    isIMUInit = true;
                    gyroTimer = 0f;
                }
            }

            // Get array data from memory
            if (gyroTimer >= gyroInterval && isIMUInit)
            {
                if (isCameraReset)
                {
                    Debug.Log("CameraReset");
                    isCameraReset = false;
                    initEulerPtr = GetEuler();
                    Marshal.Copy(initEulerPtr, initEulerArray, 0, 3);
                }
                else
                {
                    EulerPtr = GetEuler();
                    Marshal.Copy(EulerPtr, EulerArray, 0, 3); 
                    fromRotation = transform.rotation;
                    targetAngle.x = initiAngle.x + (int)(initEulerArray[1] - EulerArray[1]);
                    targetAngle.x = Mathf.Clamp(targetAngle.x, initiAngle[0] - 65.0f, initiAngle[0] + 65.0f);

                    targetAngle.y = initiAngle.y + (int)(initEulerArray[2] - EulerArray[2]);
                    targetAngle.y = Mathf.Clamp(targetAngle.y, initiAngle.y - 50.0f, initiAngle.y + 50.0f);
                    // toRotation
                    toRotation = Quaternion.Euler(targetAngle);
                }
                gyroTimer = 0f;
            }

            // FOV by UP&Down
            if (Input.GetKey(KeyCode.W))
            {
                zoomValue = 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                zoomValue = -1f;
            }
            if (zoomValue != 0)
            {
                Debug.Log("Zoom Move");
                currentFOV += zoomValue * (zoomSpeed * 2) * Time.deltaTime;
                currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);
                Camera[] cameras = Camera.allCameras;
                foreach (Camera camera in cameras)
                {
                    camera.fieldOfView = currentFOV;
                }
                zoomValue = 0f;
            }

            // Eye Distance
            if (Input.GetKey(KeyCode.A))
            {
                eyeDis = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                eyeDis = 1f;
            }
            if (eyeDis != 0)
            {
                // 只有按下A或D键时才响应
                if (isRightCamera)
                {
                    eyeDis = -eyeDis;
                }
                // 根据输入值更新水平旋转角度
                fromRotation = transform.localRotation;
                targetAngle.x = fromRotation.eulerAngles.x;
                targetAngle.y = fromRotation.eulerAngles.y + eyeDis * 200f * Time.deltaTime;
                toRotation = Quaternion.Euler(targetAngle);
                // Translate
                initiAngle.y = targetAngle.y;
                eyeDis = 0f;
            } 

            // Reset Camera
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isCameraReset = true;
                toRotation = Quaternion.Euler(initiAngle);
            }

            //float t = Mathf.Clamp01(10f * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, lerpSmoothness);
        }
    }
    private void OnApplicationQuit()
    {
        Debug.Log("Stop Nreal");
        StopConnection();
    }
}
