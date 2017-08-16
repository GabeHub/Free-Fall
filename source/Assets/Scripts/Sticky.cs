using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticky : MonoBehaviour {
    
    private GameObject player;
    private bool isSticked = false;
    private bool canStick = true;

    // Use this for initialization
    void Start () {
    }

	// Update is called once per frame
	void  Update() {
        if (isSticked)
        {            
            if ((gameObject.GetComponent<CircleCollider2D>().bounds.min.y >= player.transform.position.y + player.transform.localScale.y/2) || !player.GetComponent<Collider2D>())
            {
                gameObject.GetComponent<FixedJoint2D>().enabled = false;
                isSticked = false;
                canStick = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canStick)
        {
            isSticked = true;
            player = collision.gameObject;
            var joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
            joint.dampingRatio = 1;
        }
    }
}
