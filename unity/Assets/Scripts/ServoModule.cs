using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoModule : MonoBehaviour 
{
    public float speedCoeficient = 1.0f;

	public ServoController[] servos;

	public void Awake()
	{
        foreach (ServoController servo in servos)
        {
            servo.servoModule = this;
        }
	}

    public bool MoveTo(int servo, float angle, uint time)
    {
        if (servo < 0 || servo >= servos.Length)
            return false;

        return servos[servo].MoveTo(Mathf.Clamp(angle, -90, 90), Mathf.Clamp((float)time / 1000f, 0.001f, 10.0f));
    }
}
