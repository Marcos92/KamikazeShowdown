using UnityEngine;
using System.Collections;
using System;

public class ScrollingWalls : MonoBehaviour {

    public float speedX, speedZ;

    float fps = 0;
    public float updateFrequency;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        fps++;
        if (fps > updateFrequency)
        {
            gameObject.transform.FindChild("Cube").GetComponent<Renderer>().material.mainTextureOffset += new Vector2(speedX * Time.deltaTime, speedZ * Time.deltaTime);
            fps = 0;
        }
    }
}
