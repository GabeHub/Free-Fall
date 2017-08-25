using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour {

    public float speed;
    public GameObject player;

    private float respawnPointY = 7;
    private float timerCurrent = 0;
    private float timerPrevious = 0;
    private float timerDelta = 10;
    private CapsuleCollider2D sticks;

    // Use this for initialization
    void Start () {
        sticks = GetComponent<CapsuleCollider2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (player && player.GetComponent<Collider2D>())
        {
            timerCurrent += Time.fixedDeltaTime;
            if(timerCurrent - timerPrevious > timerDelta)
            {
                speed += 0.5f;
                timerPrevious = timerCurrent;
            }

            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime);
            //transform.Translate(Vector2.down * speed * Time.fixedDeltaTime);
            if (transform.position.y - Camera.main.transform.position.y > 7)
            {
                Respawn();
            }
        }
        else Destroy(gameObject);
	}

    void Respawn()
    {
        transform.position = new Vector2(player.transform.position.x, Camera.main.transform.position.y + respawnPointY);
    }
}
