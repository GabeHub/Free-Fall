using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swinging : MonoBehaviour {

    private HingeJoint2D joint;
    private JointMotor2D motor;
    private int direction;
    private float motorSpeed;
    private float timer;
    private float swindTime = 3.5f;

    // Use this for initialization
    void Start (){
        direction = Random.Range(0, 2);
        if (direction == 0)
        {
            direction = -1;
        }
        joint = GetComponent<HingeJoint2D>();
        motorSpeed = joint.motor.motorSpeed;
        motor = joint.motor;
        motor.motorSpeed *= direction;
        joint.motor = motor;
        timer = swindTime;
    }
	
	// Update is called once per frame
	void Update () {
        if (joint.jointAngle >= joint.limits.max)
        {
            motor.motorSpeed = -motorSpeed;
            joint.motor = motor;
            timer = swindTime;
        }
        else if (joint.jointAngle <= joint.limits.min)
        {
            motor.motorSpeed = motorSpeed;
            joint.motor = motor;
            timer = swindTime;
        }
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (joint.jointAngle>0)
            {
                motor.motorSpeed = -motorSpeed;
                joint.motor = motor;
                timer = swindTime;
            }
            else
            {
                motor.motorSpeed = motorSpeed;
                joint.motor = motor;
                timer = swindTime;
            }
        }
	}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //        joint.useMotor = false;
    //}
}
