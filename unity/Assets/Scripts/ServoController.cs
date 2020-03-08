using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoController : MonoBehaviour 
{
    public Axis axis;
    public ServoModule servoModule;

    private float currentAngle;

    private float startAngle = 0;
    private float targetAngle = 0;

    private float targetTime = 0;
    private float moveTime = 0;

    private Status status = Status.Normal;

    public enum Status
    {
        Normal,
        Busy
    }

    public enum Axis
    {
        X, Y, Z
    }

	public void Awake()
    {
        ConfigurableJoint joint = GetComponent<ConfigurableJoint>();

        JointDrive jd = new JointDrive();
        jd.maximumForce = 100000;
        jd.positionSpring = 10000;
        jd.positionDamper = 2;

        joint.angularXDrive = jd;
        joint.angularYZDrive = jd;
        //GetComponent<ConfigurableJoint>().angularYZDrive.positionSpring = 100;
        //GetComponent<ConfigurableJoint>().angularXDrive.positionSpring = 100;
	}

	public bool MoveTo(float angle, float time)
    {
        startAngle = currentAngle;
        targetAngle = angle;
        moveTime = 0;
        targetTime = time;

        status = Status.Busy;
        return true;
    }

    public void FixedUpdate()
    {
        if (status == Status.Busy)
        {
            float dt = Time.deltaTime * servoModule.speedCoeficient;

            moveTime += dt;

            float progress;

            if (moveTime >= targetTime)
            {
                moveTime = targetTime;
                targetTime = 0;
                progress = 1.0f;
                status = Status.Normal;
            }
            else
            {
                progress = moveTime / targetTime;
            }

            currentAngle = Mathf.LerpAngle(startAngle, targetAngle, progress);
        }

        Vector3 v;

        switch (axis)
        {
            case Axis.X:
            {
                v = Vector3.right;
                break;
            }
            case Axis.Y:
            {
                v = Vector3.up;
                break;
            }
            case Axis.Z:
            default:
            {
                v = Vector3.forward;
                break;
            }
        }

        GetComponent<ConfigurableJoint>().targetRotation = 
            Quaternion.AngleAxis(currentAngle, v);
    }
}
