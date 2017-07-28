using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MotionDetectorDevice : IDevice 
{
    public Vector3 gyroscope = new Vector3();
    public Vector3 accelerometer = new Vector3();

    void Update()
    {
        Rigidbody rigitBody = GetComponent<Rigidbody>();

        gyroscope.Set(
            Mathf.Floor(transform.rotation.eulerAngles.x * 100.0f) / 100.0f,
            Mathf.Floor(transform.rotation.eulerAngles.y * 100.0f) / 100.0f,
            Mathf.Floor(transform.rotation.eulerAngles.z * 100.0f) / 100.0f
        );
        accelerometer.Set(
            Mathf.Floor(rigitBody.velocity.x * 100.0f) / 100.0f,
            Mathf.Floor(rigitBody.velocity.y * 100.0f) / 100.0f,
            Mathf.Floor(rigitBody.velocity.z * 100.0f) / 100.0f
        );
    }
}
