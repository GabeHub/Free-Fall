using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour {

    public float speed;
    private float width;
    public float effectorSpeed = 5f;
    public float effectorForceScale = 0.05f;
    private SurfaceEffector2D sf;
    private int direction;
    private float borders = 0.25f;

    // Use this for initialization
    void Start () {
        width = 2.0f * Camera.main.orthographicSize * Camera.main.aspect;
        sf = GetComponent<SurfaceEffector2D>();
        sf.speed = effectorSpeed;
        sf.forceScale = effectorForceScale;
        direction = Random.Range(0, 2);
        if (direction == 0)
        {
            direction = -1;
        }
        speed *= direction;
        sf.speed *= direction;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);        
        if (gameObject.GetComponentInChildren<Collider2D>().bounds.min.x - borders < -width / 2)
        {
            speed *= -1;
            sf.speed *= -1;
        }
        else if(gameObject.GetComponentInChildren<Collider2D>().bounds.max.x + borders > width / 2)
        {
            speed *= -1;
            sf.speed *= -1;
        }
	}
}
