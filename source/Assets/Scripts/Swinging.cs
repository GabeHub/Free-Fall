using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour {

    private HingeJoint2D joint;
    private JointMotor2D motor;
    private int direction;

	// Use this for initialization
	void Start (){
        direction = Random.Range(0, 2);
        if (direction == 0)
        {
            direction = -1;
        }
        joint = GetComponent<HingeJoint2D>();
        motor = joint.motor;
        motor.motorSpeed *= direction;
        joint.motor = motor;
    }
	
	// Update is called once per frame
	void Update () {
        if (joint.jointAngle >= joint.limits.max)
        {
            motor.motorSpeed *= -1;
            joint.motor = motor;
        }
        else if (joint.jointAngle <= joint.limits.min)
        {
            motor.motorSpeed *= -1;
            joint.motor = motor;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
            joint.useMotor = false;
    }
}
