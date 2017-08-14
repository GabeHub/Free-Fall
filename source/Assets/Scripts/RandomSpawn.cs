using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour {

    public GameObject objectPrefab;

    // Use this for initialization
    void Start () {
        GameObject obj = Instantiate(objectPrefab);
        obj.transform.position = new Vector2(2, 2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
