using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

    public Gun gun;
    float timer;
    float duration = 10;

	// Use this for initialization
	void Start ()
    {
        timer = Time.time + duration;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Time.time > timer) Destroy(gameObject);
	}
}
