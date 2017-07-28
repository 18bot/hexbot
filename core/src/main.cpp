
#include <stdio.h>

#include "main.h"

HexbotPtr Hexbot::s_instance = nullptr;


int Hexbot::Create(api::LogCallback logCallback,
    api::GetGyroscopeDataCallback getGyroscopeDataCallback,
    api::GetAccelerometerDataCallback getAccelerometerDataCallback,
    api::MoveServoCallback moveServoCallback,
    api::RequestCameraSnapshotCallback requestCameraSnapshotCallback)
{
    s_instance = std::make_shared<Hexbot>(logCallback, getGyroscopeDataCallback,
        getAccelerometerDataCallback, moveServoCallback, requestCameraSnapshotCallback);
    
    return 1;
    
}

void Hexbot::log(const std::string& data)
{
    m_logCallback(data.c_str());
}

bool Hexbot::getGyroscopeData(float& x, float& y, float& z)
{
    return m_getGyroscopeDataCallback(x, y, z);
}

bool Hexbot::getAccelerometerData(float& x, float& y, float& z)
{
    return m_getAccelerometerDataCallback(x, y, z);
}

bool Hexbot::moveServo(int servo, float angle, float time)
{
    return m_moveServoCallback(servo, angle, time);
}

bool Hexbot::requestCameraSnapshot(SnapshotCallback callback)
{
    m_currentSnapshotCallback = callback;
    bool result = m_requestCameraSnapshotCallback();
    m_currentSnapshotCallback = nullptr;
    return result;
}

int Hexbot::randomInt(int a, int b)
{
    std::uniform_int_distribution<int> randomInt(a, b);
    return randomInt(m_randomGen);
}

float Hexbot::randomFloat(float a, float b)
{
    std::uniform_real_distribution<float> randomFloat(a, b);
    return randomFloat(m_randomGen);
}

void Hexbot::cameraSnapshot(int width, int height, int dataLength, void* data)
{
    if (m_currentSnapshotCallback)
        m_currentSnapshotCallback(width, height, dataLength, data);
}

void Hexbot::update(float dt)
{
    m_timer += dt;
    
    if (m_timer > 1)
    {
        m_timer = 0;
        
        requestCameraSnapshot([](int width, int height, int dataLength, void* data)
        {
            int a = 0;
        });
        
        moveServo(randomInt(0, 17), randomFloat(-90, 90), randomFloat(0.5f, 5.0f));
    }
}

Hexbot::Hexbot(api::LogCallback logCallback, api::GetGyroscopeDataCallback getGyroscopeDataCallback,
        api::GetAccelerometerDataCallback getAccelerometerDataCallback, api::MoveServoCallback moveServoCallback,
        api::RequestCameraSnapshotCallback requestCameraSnapshotCallback) :
    m_timer(0),
    m_randomGen(m_randomDevice()),

    m_logCallback(logCallback),
    m_getGyroscopeDataCallback(getGyroscopeDataCallback),
    m_getAccelerometerDataCallback(getAccelerometerDataCallback),
    m_moveServoCallback(moveServoCallback),
    m_requestCameraSnapshotCallback(requestCameraSnapshotCallback)
{
    log("Hexbot Core Initialized!");
}
