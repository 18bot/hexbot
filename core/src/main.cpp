
#include <stdio.h>

#include "main.h"
#include "animation.h"

HexbotPtr Hexbot::s_instance = nullptr;


int Hexbot::Create(
    const std::string& contentsDirectory,
    api::LogCallback logCallback,
    api::GetGyroscopeDataCallback getGyroscopeDataCallback,
    api::GetAccelerometerDataCallback getAccelerometerDataCallback,
    api::MoveServoCallback moveServoCallback,
    api::RequestCameraSnapshotCallback requestCameraSnapshotCallback)
{
    s_instance = std::make_shared<Hexbot>(contentsDirectory, logCallback, getGyroscopeDataCallback,
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
    
    if (m_timer > 0.5)
    {
        m_timer = 0;
        
        /*
        requestCameraSnapshot([](int width, int height, int dataLength, void* data)
        {
            int a = 0;
        });
        */
        
        float f = randomFloat(-90, 90);
        
        moveServo(0, 45, 1.0f);
        moveServo(1, -45, 1.0f);
        moveServo(2, -20, 1.0f);
        
        /*
        moveServo(1, -f, 1.0f);
        moveServo(4, -f, 1.0f);
        moveServo(7, -f, 1.0f);
        moveServo(10, f, 1.0f);
        moveServo(13, f, 1.0f);
        moveServo(16, f, 1.0f);
        
        moveServo(2, f, 1.0f);
        moveServo(5, f, 1.0f);
        moveServo(8, f, 1.0f);
        moveServo(11, -f, 1.0f);
        moveServo(14, -f, 1.0f);
        moveServo(17, -f, 1.0f);
        
        */
    }
}

Hexbot::Hexbot(
        const std::string& contentsDirectory,
        api::LogCallback logCallback, api::GetGyroscopeDataCallback getGyroscopeDataCallback,
        api::GetAccelerometerDataCallback getAccelerometerDataCallback, api::MoveServoCallback moveServoCallback,
        api::RequestCameraSnapshotCallback requestCameraSnapshotCallback) :
    m_timer(0),
    m_randomGen(m_randomDevice()),

    m_contentsDirectory(contentsDirectory),
    m_logCallback(logCallback),
    m_getGyroscopeDataCallback(getGyroscopeDataCallback),
    m_getAccelerometerDataCallback(getAccelerometerDataCallback),
    m_moveServoCallback(moveServoCallback),
    m_requestCameraSnapshotCallback(requestCameraSnapshotCallback)
{
    m_forwardAnimation = Animation::Create(m_contentsDirectory + "/forward.json");
    
    log("Hexbot Core Initialized!");
}
