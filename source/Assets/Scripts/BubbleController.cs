using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour {
    
    private GameObject player;
    private bool isCollected = false;
    private float timer;
    private float followingSpeed = 10.0f;
    private float timerTime = 5.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isCollected)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, followingSpeed * Time.fixedDeltaTime);
            timer -= Time.fixedDeltaTime;
        }
        if (timer < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            transform.localScale = new Vector3(player.transform.localScale.x + 0.3f, player.transform.localScale.y + 0.3f, player.transform.localScale.z);
            isCollected = true;
            timer = timerTime;
        }
    }
}
