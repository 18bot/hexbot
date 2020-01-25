
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

public class Robo : MonoBehaviour
{
    private ExposedAPI api;
    public ServoModule servoModule;

    /*
    public MotionDetectorDevice motionDetectorDevice;
    public RenderTexture cameraTexture;
    private Texture2D cameraTexture2D;
    */

    public class ExposedAPI
    {
        private IntPtr _LIB;
        private IntPtr _RoboInit;
        private IntPtr _RoboUpdate;

        public delegate int _API_RoboInit(IntPtr contentsDirectory, LogCallback logCallback, MoveServoCallback moveServoCallback);
        public delegate void _API_RoboUpdate(float dt);
        public delegate void _API_RoboCameraSnapshot(int width, int height, int dataLength, IntPtr data);

        public int RoboInit(
            IntPtr contentsDirectory,
            LogCallback logCallback,
            MoveServoCallback moveServoCallback
        )
        {
            return Native.Invoke<int, _API_RoboInit>(_LIB, _RoboInit, contentsDirectory, logCallback, moveServoCallback);
        }

        public void RoboUpdate(float dt)
        {
            Native.Invoke<_API_RoboUpdate>(_LIB, _RoboUpdate, dt);
        }

        public bool Load()
        {
            _LIB = Native.LoadLibrary(Application.dataPath + "/Plugins/hexbot.dll");

            if (_LIB == IntPtr.Zero)
            {
                return false;
            }

            Expose();

            return true;
        }

        public void Unload()
        {
            if (_LIB != IntPtr.Zero)
            {
                Native.FreeLibrary(_LIB);
            }
        }

        void Expose()
        {
            _RoboInit = Native.GetProcAddress(_LIB, "RoboInit");
            _RoboUpdate = Native.GetProcAddress(_LIB, "RoboUpdate");
        }
    }

    // delegates

    public delegate void LogCallback( IntPtr str );
    public delegate bool MoveServoCallback(int servo, float angle, float time);


    // API methods
    
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
    

    void Awake()
    {
        api = new ExposedAPI();

        if (!api.Load())
        {
            Debug.LogError("Failed to load hexbot dll!");
            return;
        }

        
        //cameraTexture2D = new Texture2D(cameraTexture.width, cameraTexture.height, TextureFormat.RGB24, false);
        
        DelegateLog = new LogCallback(InternalLog);
        DelegateMoveServo = new MoveServoCallback(InternalMoveServo);

        IntPtr contentsDirectory = Marshal.StringToHGlobalAnsi(Application.streamingAssetsPath);

        int result = api.RoboInit(
            contentsDirectory,
            DelegateLog, 
            DelegateMoveServo);

        Marshal.FreeHGlobal(contentsDirectory);

        Debug.Log("Delegate init: " + result);
    }

    void OnApplicationQuit()
    {
        api.Unload();
    }

    void FixedUpdate()
    {
        api.RoboUpdate(Time.deltaTime * servoModule.speedCoeficient);
    }
}
