using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    public float rotationSpeed = 100f;
    public float effectorSpeed = 5f;
    public float effectorForceScale = 0.025f;
    private SurfaceEffector2D sf;
    private int direction = 0;

	// Use this for initialization
	void Start () {
        if (GetComponent<SurfaceEffector2D>())
        {
            sf = GetComponent<SurfaceEffector2D>();
        }
        direction = Random.Range(0, 2);
        if (direction == 0)
        {
            direction = -1;
        }
        if (sf)
        {
            sf.speed = effectorSpeed * direction;
            sf.forceScale = effectorForceScale;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.RotateAround(transform.position, Vector3.back, rotationSpeed * direction * Time.fixedDeltaTime);
    }
}
