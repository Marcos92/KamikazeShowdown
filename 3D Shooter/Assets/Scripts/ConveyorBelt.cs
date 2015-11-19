using UnityEngine;
using System.Collections;
using System;

public class ConveyorBelt : MonoBehaviour {

    public float speedX, speedZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        gameObject.transform.FindChild("Cube").GetComponent<Renderer>().material.mainTextureOffset += new Vector2(speedX * Time.deltaTime, speedZ) * 10;
	}

    private void OnTriggerStay(Collider c)
    {
        if(c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy")
        {
            c.gameObject.transform.position += new Vector3(speedX * Time.deltaTime, 0, speedZ * Time.deltaTime) * 35;
        }
    }
}
