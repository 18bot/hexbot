using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoModule : MonoBehaviour 
{
	public ServoController[] servos;

    public bool MoveTo(int servo, float angle, float time)
    {
        if (servo < 0 || servo >= servos.Length)
            return false;

        return servos[servo].MoveTo(Mathf.Clamp(angle, -90, 90), Mathf.Clamp(time, 0.001f, 10.0f));
    }
}
