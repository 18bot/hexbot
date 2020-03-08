
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
        private IntPtr _RoboMove;
        private IntPtr _RoboStopMoving;

        public enum MoveState
        {
            MOVE_Stop = 0,
            MOVE_Forward,
            MOVE_Backward,
            MOVE_Left,
            MOVE_Right,
            MOVE_Sit
        }

        public delegate int _API_RoboInit(IntPtr contentsDirectory, LogCallback logCallback, MoveServoCallback moveServoCallback);
        public delegate void _API_RoboUpdate(uint dt);
        public delegate void _API_Move(int state);
        public delegate void _API_RoboCameraSnapshot(int width, int height, int dataLength, IntPtr data);

        public int RoboInit(
            IntPtr contentsDirectory,
            LogCallback logCallback,
            MoveServoCallback moveServoCallback
        )
        {
            return Native.Invoke<int, _API_RoboInit>(_LIB, _RoboInit, contentsDirectory, logCallback, moveServoCallback);
        }

        public void RoboUpdate(uint dt)
        {
            Native.Invoke<_API_RoboUpdate>(_LIB, _RoboUpdate, dt);
        }

        public void RoboMove(MoveState state)
        {
            Native.Invoke<_API_Move>(_LIB, _RoboMove, (int)state);
        }

        public bool Load()
        {
            _LIB = Native.LoadLibrary(Application.dataPath + "/Plugins", "hexbot");

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

            _RoboMove = Native.GetProcAddress(_LIB, "RoboMove");
        }
    }

    // delegates

    public delegate void LogCallback( IntPtr str );
    public delegate bool MoveServoCallback(int servo, float angle, uint time);


    // API methods
    
    private MoveServoCallback DelegateMoveServo;
    private bool InternalMoveServo(int servo, float angle, uint time)
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
        api.RoboUpdate((uint)(1000 * Time.deltaTime * servoModule.speedCoeficient));

        if (Input.GetKeyDown(KeyCode.W))
        {
            api.RoboMove(ExposedAPI.MoveState.MOVE_Forward);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            api.RoboMove(ExposedAPI.MoveState.MOVE_Backward);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            api.RoboMove(ExposedAPI.MoveState.MOVE_Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            api.RoboMove(ExposedAPI.MoveState.MOVE_Right);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            api.RoboMove(ExposedAPI.MoveState.MOVE_Stop);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            api.RoboMove(ExposedAPI.MoveState.MOVE_Sit);
        }
    }
}
