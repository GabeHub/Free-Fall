using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    private float distance;

	// Use this for initialization
	void Start () {
        distance = transform.position.y - target.transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        if (target && target.GetComponent<Collider2D>())
        {
            Vector3 newPosition = transform.position;
            newPosition.y = target.transform.position.y + distance;
            transform.position = newPosition;
        }
        else return;
	}
}
