﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sizing : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        //get world space size (this version operates on the bounds of the object, so expands when rotating)
        //Vector3 world_size = GetComponent<SpriteRenderer>().bounds.size;

        //get world space size (this version handles rotating correctly)
        Vector2 sprite_size = GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
        Vector3 world_size = local_sprite_size;
        world_size.x *= transform.lossyScale.x;
        world_size.y *= transform.lossyScale.y;

        //convert to screen space size
        Vector3 screen_size = 0.5f * world_size / Camera.main.orthographicSize;
        screen_size.y *= Camera.main.aspect;

        //size in pixels
        Vector3 in_pixels = new Vector3(screen_size.x * Camera.main.pixelWidth, screen_size.y * Camera.main.pixelHeight, 0) * 0.5f;

        Debug.Log(string.Format("World size: {0}, Screen size: {1}, Pixel size: {2}", world_size, screen_size, in_pixels));

    }

    // Update is called once per frame
    void Update () {
		
	}
}
