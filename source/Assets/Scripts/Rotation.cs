using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    public float rotationSpeed = 100f;
    private float effectorSpeed = 5f;
    private float effectorForceScale = 0.025f;
    private SurfaceEffector2D sf;
    public Direction direction;

	// Use this for initialization
	void Start () {
        if (GetComponent<SurfaceEffector2D>())
        {
            sf = GetComponent<SurfaceEffector2D>();
        }
        if (sf)
        {
            sf.speed = effectorSpeed * (int)direction;
            sf.forceScale = effectorForceScale;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.RotateAround(transform.position, Vector3.back, rotationSpeed * (int)direction * Time.fixedDeltaTime);
    }
}
