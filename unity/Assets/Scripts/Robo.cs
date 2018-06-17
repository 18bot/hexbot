
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Robo : MonoBehaviour 
{
    public ServoModule servoModule;
    public MotionDetectorDevice motionDetectorDevice;
    public RenderTexture cameraTexture;

    private Texture2D cameraTexture2D;

    // delegates

    public delegate void LogCallback( IntPtr str );
    public delegate bool GetGyroscopeDataCallback( ref float x, ref float y, ref float z );
    public delegate bool GetAccelerometerDataCallback( ref float x, ref float y, ref float z );
    public delegate bool MoveServoCallback(int servo, float angle, float time);
    public delegate bool RequestCameraSnapshotCallback();

    // API methods

    [DllImport ("hexbot")]
    private static extern int RoboInit(
        IntPtr contentsDirectory,
        LogCallback logCallback,
        GetGyroscopeDataCallback getGyroscopeDataCallback,
        GetAccelerometerDataCallback getAccelerometerDataCallback,
        MoveServoCallback moveServoCallback,
        RequestCameraSnapshotCallback requestCameraSnapshotCallback
    );

    [DllImport ("hexbot")]
    private static extern void RoboUpdate(float dt);

    [DllImport ("hexbot")]
    private static extern void RoboCameraSnapshot(int width, int height, int dataLength, IntPtr data);

    private RequestCameraSnapshotCallback DelegateRequestCameraSnapshot;
    private unsafe bool InternalRequestCameraSnapshot()
    {
        RenderTexture.active = cameraTexture;
        cameraTexture2D.ReadPixels(new Rect(0, 0, cameraTexture.width, cameraTexture.height), 0, 0);
        cameraTexture2D.Apply();

        byte[] rawData = cameraTexture2D.GetRawTextureData();

        fixed (byte* p = rawData)
        {
            IntPtr ptr = (IntPtr)p;
            RoboCameraSnapshot(cameraTexture.width, cameraTexture.height, rawData.Length, ptr);
        }

        return true;
    }

    private MoveServoCallback DelegateMoveServo;
    private bool InternalMoveServo(int servo, float angle, float time)
    {
        return servoModule.MoveTo(servo, angle, time);
    }

    private LogCallback DelegateLog;
    private void InternalLog( IntPtr str )
    {
        Debug.Log(Marshal.PtrToStringAnsi(str));
    }

    private GetAccelerometerDataCallback DelegateGetAccelerometerData;
    private bool InternalGetAccelerometerData( ref float x, ref float y, ref float z )
    {
        Vector3 data = motionDetectorDevice.accelerometer;

        x = data.x;
        y = data.y;
        z = data.z;

        return true;
    }

    private GetGyroscopeDataCallback DelegateGetGyroscopeData;
    private bool InternalGetGyroscopeData( ref float x, ref float y, ref float z )
    {
        Vector3 data = motionDetectorDevice.gyroscope;

        x = data.x;
        y = data.y;
        z = data.z;

        return true;
    }

    void Awake() 
    {
        cameraTexture2D = new Texture2D(cameraTexture.width, cameraTexture.height, TextureFormat.RGB24, false);

        DelegateLog = new LogCallback(InternalLog);
        DelegateGetGyroscopeData = new GetGyroscopeDataCallback(InternalGetGyroscopeData);
        DelegateGetAccelerometerData = new GetAccelerometerDataCallback(InternalGetAccelerometerData);
        DelegateMoveServo = new MoveServoCallback(InternalMoveServo);
        DelegateRequestCameraSnapshot = new RequestCameraSnapshotCallback(InternalRequestCameraSnapshot);

#if UNITY_EDITOR
        String contentsPath = Application.dataPath + "/StreamingAssets";
#else
        String contentsPath = Application.dataPath + "/Resources/Data/StreamingAssets";
#endif

        IntPtr contentsDirectory = Marshal.StringToHGlobalAnsi(contentsPath);

        int result = RoboInit(
            contentsDirectory,
            DelegateLog, 
            DelegateGetGyroscopeData, 
            DelegateGetAccelerometerData, 
            DelegateMoveServo,
            DelegateRequestCameraSnapshot);

        Marshal.FreeHGlobal(contentsDirectory);

        Debug.Log("Delegate init: " + result);
    }

    void Update()
    {
        RoboUpdate(Time.deltaTime);
    }
}
