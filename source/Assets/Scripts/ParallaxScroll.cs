using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour {
    
    public Renderer background;
    public float backgroundSpeed = 0.02f;
    public float offset = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
         float backgroundOffset = offset * backgroundSpeed;
         background.material.mainTextureOffset = new Vector2(0, backgroundOffset);
    }
}
