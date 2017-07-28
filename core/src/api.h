
#ifndef HEXBOT_API
#define HEXBOT_API

// callbacks

namespace api
{
    typedef void (*LogCallback) (const char*);
    typedef bool (*GetGyroscopeDataCallback) (float& x, float& y, float& z);
    typedef bool (*GetAccelerometerDataCallback) (float& x, float& y, float& z);
    typedef bool (*MoveServoCallback) (int servo, float angle, float time);
    typedef bool (*RequestCameraSnapshotCallback) ();
};

// api itself

extern "C"
{
    int RoboInit(
        api::LogCallback logCallback,
        api::GetGyroscopeDataCallback getGyroscopeDataCallback,
        api::GetAccelerometerDataCallback getAccelerometerDataCallback,
        api::MoveServoCallback moveServoCallback, api::RequestCameraSnapshotCallback requestCameraSnapshotCallback
    );
    
    void RoboUpdate(float dt);
    
    void RoboCameraSnapshot(int width, int height, int dataLength, void* data);
}

#endif
